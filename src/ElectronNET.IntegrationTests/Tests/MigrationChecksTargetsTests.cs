using System.Diagnostics;

namespace ElectronNET.IntegrationTests.Tests;

/// <summary>
/// Unit tests for ElectronNET.MigrationChecks.targets - no Electron runtime required.
/// Covers GitHub issue #1035: System.IO.File.ReadAllLines is not available as an MSBuild
/// property function on all platforms (e.g. macOS GitHub Actions), causing MSB4185.
/// </summary>
public class MigrationChecksTargetsTests
{
    private static readonly string TargetsFilePath = FindTargetsFile();

    /// <summary>
    /// Walks up the directory tree from <see cref="AppContext.BaseDirectory"/> until it finds
    /// the migration checks targets file. This is robust against varying output paths
    /// (with or without RID subfolder, debug/release, etc.).
    /// </summary>
    private static string FindTargetsFile()
    {
        const string RelativeFromRepoRoot =
            "src/ElectronNET/build/ElectronNET.MigrationChecks.targets";
        const string RelativeFromSrc =
            "ElectronNET/build/ElectronNET.MigrationChecks.targets";

        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var fromRepoRoot = Path.Combine(dir.FullName, RelativeFromRepoRoot);
            if (File.Exists(fromRepoRoot))
            {
                return Path.GetFullPath(fromRepoRoot);
            }

            var fromSrc = Path.Combine(dir.FullName, RelativeFromSrc);
            if (File.Exists(fromSrc))
            {
                return Path.GetFullPath(fromSrc);
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            "Could not locate ElectronNET.MigrationChecks.targets by walking up from " +
            $"'{AppContext.BaseDirectory}'.");
    }

    // -----------------------------------------------------------------------
    // Content-level test (RED before fix, GREEN after fix on ALL platforms)
    // -----------------------------------------------------------------------

    [Fact]
    public void MigrationChecksTargets_ShouldNotUseReadAllLines()
    {
        // The file must exist - if this fails the path constant above is wrong.
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
    // Functional build test - verifies no MSB4185 at runtime
    // (RED on platforms where ReadAllLines is restricted, GREEN after fix)
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MigrationChecksTargets_BuildWithCleanPackageJson_ShouldSucceedWithoutMSB4185()
    {
        // Positive case: a package.json that does NOT mention electron.
        // The migration check must successfully read the file via ReadAllText
        // (the code path fixed by issue #1035) without producing MSB4185.

        var tempDir = CreateTempProjectDirectory();
        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(tempDir, "package.json"),
                """{ "devDependencies": { "vite": "^5.0.0" } }""");

            await WriteMinimalCsprojAsync(tempDir);

            var (exitCode, output) = await RunDotnetBuildAsync(tempDir);

            exitCode.Should().Be(0,
                $"the build must succeed when the package.json contains no electron references. " +
                $"Full build output:\n{output}");

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

    [Fact]
    public async Task MigrationChecksTargets_BuildWithPackageJsonContainingElectron_ShouldEmitELECTRON008WarningWithoutMSB4185()
    {
        // Negative case: a package.json that DOES contain "electron".
        // The migration check must still read the file successfully (no MSB4185)
        // and must emit the expected ELECTRON008 warning. ELECTRON008 is a
        // <Warning>, not an <Error>, so the build itself still succeeds.

        var tempDir = CreateTempProjectDirectory();
        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(tempDir, "package.json"),
                """{ "devDependencies": { "electron": "^30.0.0" } }""");

            await WriteMinimalCsprojAsync(tempDir);

            var (exitCode, output) = await RunDotnetBuildAsync(tempDir);

            exitCode.Should().Be(0,
                $"ELECTRON008 is a Warning (not an Error) so the build itself must still " +
                $"succeed. Full build output:\n{output}");

            output.Should().NotContain(
                "MSB4185",
                $"ReadAllLines must not be used as an MSBuild property function. " +
                $"Full build output:\n{output}");

            output.Should().Contain(
                "ELECTRON008",
                $"the migration check must still detect electron references in package.json " +
                $"after the ReadAllText migration. Full build output:\n{output}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static string CreateTempProjectDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"electron-net-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    private static Task WriteMinimalCsprojAsync(string tempDir)
    {
        // A minimal csproj that only imports the migration checks targets to keep the
        // build fast. Note: MSBuildProjectDirectory is a reserved MSBuild property and
        // must not be redefined manually; MSBuild sets it automatically to the folder
        // of the csproj (which is tempDir here).
        var targetsPathEscaped = TargetsFilePath.Replace("'", "&apos;");
        return File.WriteAllTextAsync(
            Path.Combine(tempDir, "TestApp.csproj"),
            $"""
            <Project>
              <Import Project="{targetsPathEscaped}" />
              <Target Name="Build" DependsOnTargets="ElectronMigrationChecks" />
            </Project>
            """);
    }

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
