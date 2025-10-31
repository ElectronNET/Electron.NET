using Microsoft.Build.Exceptions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Octokit.Internal;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
// ReSharper disable ArrangeThisQualifier

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.RunUnitTests);

    [Nuke.Common.Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Nuke.Common.Parameter("ReleaseNotesFilePath - To determine the lates changelog version")]
    readonly AbsolutePath ReleaseNotesFilePath = RootDirectory / "Changelog.md";

    [Nuke.Common.Parameter("common.props file path - to determine the configured version")]
    readonly AbsolutePath CommonPropsFilePath = RootDirectory / "src" / "common.props";

    [Solution]
    readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath ResultDirectory => RootDirectory / "artifacts";

    GitHubActions GitHubActions => GitHubActions.Instance;

    IReadOnlyList<ReleaseNotes> ChangeLog { get; set; }

    ReleaseNotes LatestReleaseNotes { get; set; }

    SemVersion SemVersion { get; set; }

    string Version { get; set; }

    string VersionPostFix { get; set; }

    protected override void OnBuildInitialized()
    {
        var parser = new ReleaseNotesParser();

        Log.Debug("Reading ChangeLog {FilePath}...", ReleaseNotesFilePath);
        ChangeLog = parser.Parse(File.ReadAllText(ReleaseNotesFilePath));
        ChangeLog.NotNull("ChangeLog / ReleaseNotes could not be read!");

        LatestReleaseNotes = ChangeLog.First();
        LatestReleaseNotes.NotNull("LatestVersion could not be read!");

        var propsParser = new CommonPropsParser();

        var propsVersion = propsParser.Parse(CommonPropsFilePath);

        propsVersion.NotNull("Version from common.props could not be read!");

        Assert.True(propsVersion == LatestReleaseNotes.Version,
                $"The version in common.props ({propsVersion}) does not " +
                $"equal the latest version in the changelog ({LatestReleaseNotes.Version})");

        Log.Debug("Using version: {LatestVersion}", propsVersion);
        SemVersion = LatestReleaseNotes.SemVersion;
        Version = propsVersion.ToString();

        if (GitHubActions != null)
        {
            Log.Debug("Add Version Postfix if under CI - GithubAction(s)...");

            var buildNumber = GitHubActions.RunNumber;

            if (ScheduledTargets.Contains(Default))
            {
                VersionPostFix = $"-ci.{buildNumber}";
            }
            else if (ScheduledTargets.Contains(PrePublish))
            {
                VersionPostFix = $"-pre.{buildNumber}";
            }
        }
        else if (ScheduledTargets.Contains(PrePublish))
        {
            VersionPostFix = $"-pre";
        }

        Log.Information("Building version {Version} with postfix {VersionPostFix}", Version, VersionPostFix);
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(dir => dir.DeleteDirectory());
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution.Path));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution.Path)
                .SetConfiguration(Configuration)
                .SetProperty("GeneratePackageOnBuild", "True")
                .SetProperty("VersionPostFix", VersionPostFix ?? string.Empty));
        });

    Target RunUnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // There aren't any yet
        });

    Target CreatePackages => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Packages are created on build
        });

    Target PublishPackages => _ => _
        .DependsOn(CreatePackages)
        .DependsOn(RunUnitTests)
        .Executes(() =>
        {
            var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");


            if (apiKey.IsNullOrEmpty())
            {
                throw new BuildAbortedException("Could not resolve the NuGet API key.");
            }

            foreach (var nupkg in ResultDirectory.GlobFiles("*.nupkg"))
            {
                DotNetNuGetPush(s => s
                    .SetTargetPath(nupkg)
                    .SetSource("https://api.nuget.org/v3/index.json")
                    .SetApiKey(apiKey));
            }
        });

    Target PublishPreRelease => _ => _
        .DependsOn(PublishPackages)
        .Executes(() =>
        {
            string gitHubToken;

            if (GitHubActions != null)
            {
                gitHubToken = GitHubActions.Token;
            }
            else
            {
                gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            }

            if (gitHubToken.IsNullOrEmpty())
            {
                throw new BuildAbortedException("Could not resolve GitHub token.");
            }

            var credentials = new Credentials(gitHubToken);

            GitHubTasks.GitHubClient = new GitHubClient(
                new ProductHeaderValue(nameof(NukeBuild)),
                new InMemoryCredentialStore(credentials));

            GitHubTasks.GitHubClient.Repository.Release
                .Create("ElectronNET", "Electron.NET", new NewRelease(Version + VersionPostFix)
                {
                    Name = "ElectronNET.Core " + Version + VersionPostFix,
                    Body = String.Join(Environment.NewLine, LatestReleaseNotes.Notes),
                    Prerelease = true,
                    TargetCommitish = "develop",
                });
        });

    Target PublishRelease => _ => _
        .DependsOn(PublishPackages)
        .Executes(() =>
        {
            string gitHubToken;

            if (GitHubActions != null)
            {
                gitHubToken = GitHubActions.Token;
            }
            else
            {
                gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            }

            if (gitHubToken.IsNullOrEmpty())
            {
                throw new BuildAbortedException("Could not resolve GitHub token.");
            }

            var credentials = new Credentials(gitHubToken);

            GitHubTasks.GitHubClient = new GitHubClient(
                new ProductHeaderValue(nameof(NukeBuild)),
                new InMemoryCredentialStore(credentials));

            GitHubTasks.GitHubClient.Repository.Release
                .Create("ElectronNET", "Electron.NET", new NewRelease(Version)
                {
                    Name = "ElectronNET.Core " + Version,
                    Body = String.Join(Environment.NewLine, LatestReleaseNotes.Notes),
                    Prerelease = false,
                    TargetCommitish = "main",
                });
        });

    Target Package => _ => _
        .DependsOn(RunUnitTests)
        .DependsOn(CreatePackages);

    Target Default => _ => _
        .DependsOn(Package);

    Target Publish => _ => _
        .DependsOn(PublishRelease);

    Target PrePublish => _ => _
        .DependsOn(PublishPreRelease);
}
