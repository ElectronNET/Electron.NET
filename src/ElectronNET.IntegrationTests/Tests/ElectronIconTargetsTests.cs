using System.Diagnostics;

namespace ElectronNET.IntegrationTests.Tests;

/// <summary>
/// Tests for ElectronNET.Core.targets icon copy behavior.
/// Covers GitHub issue #1047: modern macOS .icon app-icon packages are folders
/// and must be copied recursively, not treated as a single file.
/// </summary>
public class ElectronIconTargetsTests
{
    private static readonly string CorePropsPath = FindBuildFile("src/ElectronNET/build/ElectronNET.Core.props", "ElectronNET/build/ElectronNET.Core.props");
    private static readonly string CoreTargetsPath = FindBuildFile("src/ElectronNET/build/ElectronNET.Core.targets", "ElectronNET/build/ElectronNET.Core.targets");

    [Fact]
    public async Task ElectronCoreTargets_ElectronIconDirectory_ShouldBeMappedIntoElectronOutput()
    {
        var tempDir = CreateTempProjectDirectory();
        try
        {
            var iconPackageDir = Path.Combine(tempDir, "Assets", "MyApp.icon");
            Directory.CreateDirectory(Path.Combine(iconPackageDir, "layers"));

            await File.WriteAllTextAsync(Path.Combine(iconPackageDir, "manifest.json"), "{}");
            await File.WriteAllTextAsync(Path.Combine(iconPackageDir, "layers", "foreground.png"), "not-a-real-png");

            await WriteMinimalCsprojAsync(tempDir);

            var (exitCode, output) = await RunDotnetMsBuildAsync(tempDir, "DumpElectronIconCopyItems");
            var normalizedOutput = NormalizePathSeparators(output);

            exitCode.Should().Be(0,
                $"MSBuild target evaluation must succeed. Full output:\n{output}");

            normalizedOutput.Should().Contain(
                ".electron/MyApp.icon/manifest.json",
                $"the icon package root file must be mapped into .electron/MyApp.icon. Full output:\n{output}");

            normalizedOutput.Should().Contain(
                ".electron/MyApp.icon/layers/foreground.png",
                $"nested files in .icon package must preserve structure in .electron output. Full output:\n{output}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    private static string FindBuildFile(string relativeFromRepoRoot, string relativeFromSrc)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var fromRepoRoot = Path.Combine(dir.FullName, relativeFromRepoRoot);
            if (File.Exists(fromRepoRoot))
            {
                return Path.GetFullPath(fromRepoRoot);
            }

            var fromSrc = Path.Combine(dir.FullName, relativeFromSrc);
            if (File.Exists(fromSrc))
            {
                return Path.GetFullPath(fromSrc);
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"Could not locate '{relativeFromRepoRoot}' by walking up from '{AppContext.BaseDirectory}'.");
    }

    private static string CreateTempProjectDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"electron-net-icon-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        Directory.CreateDirectory(Path.Combine(tempDir, "Properties"));
        return tempDir;
    }

    private static Task WriteMinimalCsprojAsync(string tempDir)
    {
        var propsPathEscaped = CorePropsPath.Replace("'", "&apos;");
        var targetsPathEscaped = CoreTargetsPath.Replace("'", "&apos;");

        return File.WriteAllTextAsync(
            Path.Combine(tempDir, "TestApp.csproj"),
                        $$"""
            <Project Sdk="Microsoft.NET.Sdk">
                            <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>

              <Import Project="{{propsPathEscaped}}" />

                            <PropertyGroup Label="ElectronNetCommon">
                                <ElectronIcon>Assets/MyApp.icon</ElectronIcon>
                            </PropertyGroup>

              <Import Project="{{targetsPathEscaped}}" />

              <Target Name="DumpElectronIconCopyItems"
                      DependsOnTargets="ElectronResolvePaths;ElectronGetCopyToOutputDirectoryItems">
                <Message Importance="High"
                         Text="ELECTRON_COPY_ITEMS: @(_ElectronFilesToCopyWithTargetPath->'%(TargetPath)')" />
              </Target>
            </Project>
            """);
    }

    private static async Task<(int ExitCode, string Output)> RunDotnetMsBuildAsync(string workingDirectory, string target)
    {
        var psi = new ProcessStartInfo("dotnet", $"msbuild TestApp.csproj --nologo -v:minimal /restore /t:{target}")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(psi)!;
        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (process.ExitCode, stdOut + stdErr);
    }

    private static string NormalizePathSeparators(string value)
    {
        return value.Replace('\\', '/');
    }
}
