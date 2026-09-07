using System.Diagnostics;

namespace ElectronNET.IntegrationTests.Tests;

/// <summary>
/// Tests for the Electron clean targets.
/// Covers GitHub issue #1096: cleaning must not require a restore, so the clean targets
/// must not depend on targets that need the assets file (NETSDK1004).
/// </summary>
public class ElectronCleanTargetsTests
{
    private static readonly string CorePropsPath = FindBuildFile("src/ElectronNET/build/ElectronNET.Core.props", "ElectronNET/build/ElectronNET.Core.props");
    private static readonly string CoreTargetsPath = FindBuildFile("src/ElectronNET/build/ElectronNET.Core.targets", "ElectronNET/build/ElectronNET.Core.targets");

    [Fact]
    public async Task ElectronCleanTargets_WithoutRestore_ShouldSucceed()
    {
        var tempDir = CreateTempProjectDirectory();

        try
        {
            await WriteMinimalCsprojAsync(tempDir);

            var (exitCode, output) = await RunDotnetMsBuildAsync(tempDir, "Clean");

            output.Should().NotContain("NETSDK1004",
                $"cleaning must not require the assets file of a previous restore. Full output:\n{output}");

            exitCode.Should().Be(0,
                $"cleaning an unrestored project must succeed. Full output:\n{output}");
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
        var tempDir = Path.Combine(Path.GetTempPath(), $"electron-net-clean-test-{Guid.NewGuid():N}");
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

              <Import Project="{{targetsPathEscaped}}" />
            </Project>
            """);
    }

    private static async Task<(int ExitCode, string Output)> RunDotnetMsBuildAsync(string workingDirectory, string target)
    {
        // Deliberately without /restore - that is what issue #1096 is about.
        var psi = new ProcessStartInfo("dotnet", $"msbuild TestApp.csproj --nologo -v:minimal /t:{target}")
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
}
