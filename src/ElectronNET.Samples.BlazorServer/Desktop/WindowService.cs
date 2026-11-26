using ElectronNET.API;

namespace ElectronNET.Samples.BlazorServer.Desktop;

/// <summary>
/// Electron control: everything the UI may do with the main window.
/// </summary>
/// <remarks>
/// Components only talk to this service - never to <c>Electron.*</c> directly. This keeps the
/// Electron specifics in one place and lets the UI work (with reduced functionality) when the app
/// is opened in a normal browser, where <see cref="IsAvailable"/> is <c>false</c>.
/// </remarks>
public sealed class WindowService(AppEvents events)
{
    private BrowserWindow? window;

    public bool IsAvailable => this.window is not null;

    public bool IsMaximized { get; private set; }

    public bool IsFocused { get; private set; } = true;

    public bool IsAlwaysOnTop { get; private set; }

    /// <summary>
    /// Called once by <see cref="ElectronStartup"/> after the main window was created.
    /// </summary>
    public void Attach(BrowserWindow mainWindow)
    {
        this.window = mainWindow;

        // Electron -> .NET: keep our state in sync and forward it to the UI.
        mainWindow.OnMaximize += () => this.Update(() => this.IsMaximized = true);
        mainWindow.OnUnmaximize += () => this.Update(() => this.IsMaximized = false);
        mainWindow.OnFocus += () => this.Update(() => this.IsFocused = true);
        mainWindow.OnBlur += () => this.Update(() => this.IsFocused = false);
        mainWindow.OnClosed += () => this.window = null;
    }

    public void SetTitle(string title) => this.window?.SetTitle(title);

    public void Minimize() => this.window?.Minimize();

    public void ToggleMaximize()
    {
        if (this.IsMaximized)
        {
            this.window?.Unmaximize();
        }
        else
        {
            this.window?.Maximize();
        }
    }

    public void ToggleAlwaysOnTop()
    {
        if (this.window is null)
        {
            return;
        }

        this.IsAlwaysOnTop = !this.IsAlwaysOnTop;
        this.window.SetAlwaysOnTop(this.IsAlwaysOnTop);
        events.NotifyWindowStateChanged();
    }

    /// <summary>
    /// Shows progress in the taskbar (Windows / Linux) or dock (macOS). Use a negative value to remove it.
    /// </summary>
    public void SetProgress(double progress) => this.window?.SetProgressBar(progress);

    private void Update(Action change)
    {
        change();
        events.NotifyWindowStateChanged();
    }
}
