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

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.RunUnitTests);

    [Nuke.Common.Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Nuke.Common.Parameter("ReleaseNotesFilePath - To determine the SemanticVersion")]
    readonly AbsolutePath ReleaseNotesFilePath = RootDirectory / "Changelog.md";

    [Solution]
    readonly Solution Solution;

    string TargetProjectName => "ElectronNET";

    string ApiTargetLibName => $"{TargetProjectName}.API";

    string CliTargetLibName => $"{TargetProjectName}.CLI";

    string DemoTargetLibName => $"{TargetProjectName}.WebApp";

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath ResultDirectory => RootDirectory / "artifacts";

    GitHubActions GitHubActions => GitHubActions.Instance;

    IReadOnlyList<ReleaseNotes> ChangeLog { get; set; }

    ReleaseNotes LatestReleaseNotes { get; set; }

    SemVersion SemVersion { get; set; }

    string Version { get; set; }

    AbsolutePath[] Projects
    {
        get
        {
            var api = SourceDirectory / ApiTargetLibName / $"{ApiTargetLibName}.csproj";
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var projects = new[] { api, cli };
            return projects;    
        }        
    }

    protected override void OnBuildInitialized()
    {
        var parser = new ReleaseNotesParser();

        Log.Debug("Reading ChangeLog {FilePath}...", ReleaseNotesFilePath);
        ChangeLog = parser.Parse(File.ReadAllText(ReleaseNotesFilePath));
        ChangeLog.NotNull("ChangeLog / ReleaseNotes could not be read!");

        LatestReleaseNotes = ChangeLog.First();
        LatestReleaseNotes.NotNull("LatestVersion could not be read!");

        Log.Debug("Using LastestVersion from ChangeLog: {LatestVersion}", LatestReleaseNotes.Version);
        SemVersion = LatestReleaseNotes.SemVersion;
        Version = LatestReleaseNotes.Version.ToString();

        if (GitHubActions != null)
        {
            Log.Debug("Add Version Postfix if under CI - GithubAction(s)...");

            var buildNumber = GitHubActions.RunNumber;

            if (ScheduledTargets.Contains(Default))
            {
                Version = $"{Version}-ci.{buildNumber}";
            }
            else if (ScheduledTargets.Contains(PrePublish))
            {
                Version = $"{Version}-alpha.{buildNumber}";
            }
        }

        Log.Information("Building version: {Version}", Version);
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
            Projects.ForEach(project =>
            {
                DotNetRestore(s => s
                    .SetProjectFile(project));
            });
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Projects.ForEach(project =>
            {
                DotNetBuild(s => s
                    .SetProjectFile(project)
                    .SetVersion(Version)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore());
            });
        });

    Target RunUnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Projects.ForEach(project =>
            {
                DotNetTest(s => s
                    .SetProjectFile(project)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .EnableNoBuild());
            });
        });

    Target CreatePackages => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Projects.ForEach(project =>
            {
                DotNetPack(s => s
                    .SetProject(project)
                    .SetVersion(Version)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ResultDirectory)
                    .SetIncludeSymbols(true)
                    .SetSymbolPackageFormat("snupkg")
                    .EnableNoRestore()
                );
            });
        });

    Target CompileSample => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName / $"{DemoTargetLibName}.csproj";
            DotNetBuild(s => s.SetProjectFile(sample).SetConfiguration(Configuration));
        });

    Target ElectronizeGenericTargetSample => _ => _
        .DependsOn(CompileSample)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName;
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var args = "build /target custom win7-x86;win /dotnet-configuration Debug /electron-arch ia32  /electron-params \"--publish never\"";

            DotNet($"run --project {cli} -- {args}", sample);
        });

    Target ElectronizeWindowsTargetSample => _ => _
        .DependsOn(CompileSample)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName;
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var args = "build /target win /electron-params \"--publish never\"";

            DotNet($"run --project {cli} -- {args}", sample);
        });

    Target ElectronizeCustomWin7TargetSample => _ => _
        .DependsOn(CompileSample)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName;
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var args = "build /target custom win7-x86;win /electron-params \"--publish never\"";

            DotNet($"run --project {cli} -- {args}", sample);
        });

    Target ElectronizeMacOsTargetSample => _ => _
        .DependsOn(CompileSample)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName;
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var args = "build /target osx /electron-params \"--publish never\"";

            DotNet($"run --project {cli} -- {args}", sample);
        });

    Target ElectronizeLinuxTargetSample => _ => _
        .DependsOn(CompileSample)
        .Executes(() =>
        {
            var sample = SourceDirectory / DemoTargetLibName;
            var cli = SourceDirectory / CliTargetLibName / $"{CliTargetLibName}.csproj";
            var args = "build /target linux /electron-params \"--publish never\"";

            DotNet($"run --project {cli} -- {args}", sample);
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
                .Create("ElectronNET", "Electron.NET", new NewRelease(Version)
                {
                    Name = Version,
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
                    Name = Version,
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
