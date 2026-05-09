using System.Diagnostics;

namespace ElectronNET.IntegrationTests.Tests;

/// <summary>
/// Unit tests for ElectronNET.MigrationChecks.targets — no Electron runtime required.
/// Covers GitHub issue #1035: System.IO.File.ReadAllLines is not available as an MSBuild
/// property function on all platforms (e.g. macOS GitHub Actions), causing MSB4185.
/// </summary>
public class MigrationChecksTargetsTests
{
    // AppContext.BaseDirectory resolves to:
    //   src\ElectronNET.IntegrationTests\bin\Debug\net10.0\win-x64\
    // Five levels up => src\
    private static readonly string TargetsFilePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "ElectronNET", "build", "ElectronNET.MigrationChecks.targets"));

    // -----------------------------------------------------------------------
    // Content-level test (RED before fix, GREEN after fix on ALL platforms)
    // -----------------------------------------------------------------------

    [Fact]
    public void MigrationChecksTargets_ShouldNotUseReadAllLines()
    {
        // The file must exist — if this fails the path constant above is wrong.
        File.Exists(TargetsFilePath).Should().BeTrue(
            $"targets file must exist at '{TargetsFilePath}'");

        var content = File.ReadAllText(TargetsFilePath);

        // System.IO.File::ReadAllLines is not in the MSBuild property-function
        // whitelist on all platforms (MSB4185 on macOS GitHub Actions, see #1035).
        // ReadAllText must be used instead.
        content.Should().NotContain(
            "::ReadAllLines(",
            "because ReadAllLines is not available as an MSBuild property function on all " +
            "platforms. Use ReadAllText instead (GitHub issue #1035).");
    }

    // -----------------------------------------------------------------------
    // Functional build test — verifies no MSB4185 at runtime
    // (RED on platforms where ReadAllLines is restricted, GREEN after fix)
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MigrationChecksTargets_BuildWithPackageJsonContainingElectron_ShouldSucceedWithoutMSB4185()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"electron-net-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Create a minimal package.json that contains "electron" so ELECTRON008 fires.
            await File.WriteAllTextAsync(
                Path.Combine(tempDir, "package.json"),
                """{ "devDependencies": { "electron": "^30.0.0" } }""");

            // Create a minimal csproj that only imports the migration checks targets.
            // We deliberately import just that one targets file to keep the build fast.
            // Note: MSBuildProjectDirectory is a reserved MSBuild property — it must not be
            // redefined manually. MSBuild sets it automatically to the csproj's folder (tempDir).
            var targetsPathEscaped = TargetsFilePath.Replace("'", "&apos;");
            await File.WriteAllTextAsync(
                Path.Combine(tempDir, "TestApp.csproj"),
                $"""
                <Project>
                  <Import Project="{targetsPathEscaped}" />
                  <Target Name="Build" DependsOnTargets="ElectronMigrationChecks" />
                </Project>
                """);

            // ACT — run the Build target
            var (exitCode, output) = await RunDotnetBuildAsync(tempDir);

            // ASSERT — the build must succeed and must not produce MSB4185
            exitCode.Should().Be(0,
                $"the temporary MSBuild project should build successfully. Full build output:\n{output}");

            output.Should().NotContain(
                "MSB4185",
                $"ReadAllLines must not be used as an MSBuild property function. " +
                $"Full build output:\n{output}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    // -----------------------------------------------------------------------
    // Helper
    // -----------------------------------------------------------------------

    private static async Task<(int ExitCode, string Output)> RunDotnetBuildAsync(string workingDirectory)
    {
        var psi = new ProcessStartInfo("dotnet", "build --nologo -v:minimal")
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
