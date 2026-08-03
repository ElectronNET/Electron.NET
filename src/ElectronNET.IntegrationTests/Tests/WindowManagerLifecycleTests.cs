namespace ElectronNET.IntegrationTests.Tests;

/// <summary>
/// Regression checks for BrowserWindow lifecycle cleanup in WindowManager.
/// Covers GitHub issue #1008.
/// </summary>
public class WindowManagerLifecycleTests
{
    private static readonly string WindowManagerFilePath = FindWindowManagerFile();

    [Fact]
    public void WindowManager_ShouldUsePersistentBrowserWindowClosedSubscription()
    {
        File.Exists(WindowManagerFilePath).Should().BeTrue(
            $"WindowManager source must exist at '{WindowManagerFilePath}'.");

        var content = File.ReadAllText(WindowManagerFilePath);

        content.Should().Contain(
            "Socket.On<int[]>(\"BrowserWindowClosed\"",
            "closed-window cleanup must be wired for every close event, not just the first one.");

        content.Should().NotContain(
            "Socket.Once<int[]>(\"BrowserWindowClosed\"",
            "a one-shot subscription causes stale BrowserWindow references after subsequent closes (issue #1008).");
    }

    private static string FindWindowManagerFile()
    {
        const string RelativeFromRepoRoot = "src/ElectronNET.API/API/WindowManager.cs";
        const string RelativeFromSrc = "ElectronNET.API/API/WindowManager.cs";

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
            "Could not locate WindowManager.cs by walking up from " +
            $"'{AppContext.BaseDirectory}'.");
    }
}
