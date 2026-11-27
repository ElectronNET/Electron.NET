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
    private readonly object sync = new();
    private BrowserWindow? window;

    public bool IsAvailable => this.window is not null;

    public bool IsMaximized { get; private set; }

    public bool IsFocused { get; private set; } = true;

    public bool IsAlwaysOnTop { get; private set; }

    /// <summary>
    /// Called by <see cref="ElectronStartup"/> whenever the main window was created - initially and
    /// again when Electron recreates it (macOS: app activated after its last window was closed).
    /// </summary>
    public void Attach(BrowserWindow mainWindow)
    {
        lock (this.sync)
        {
            if (ReferenceEquals(this.window, mainWindow))
            {
                return;
            }

            this.DetachCore();

            this.window = mainWindow;

            // A new window always starts out in its default state.
            this.IsMaximized = false;
            this.IsFocused = true;
            this.IsAlwaysOnTop = false;

            // Electron -> .NET: keep our state in sync and forward it to the UI.
            mainWindow.OnMaximize += this.HandleMaximize;
            mainWindow.OnUnmaximize += this.HandleUnmaximize;
            mainWindow.OnFocus += this.HandleFocus;
            mainWindow.OnBlur += this.HandleBlur;
            mainWindow.OnClosed += this.HandleClosed;
        }

        // Let the UI pick up the (re)attached window, e.g. to enable the Window Control buttons again.
        events.NotifyWindowStateChanged();
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

    private void HandleMaximize() => this.Update(() => this.IsMaximized = true);

    private void HandleUnmaximize() => this.Update(() => this.IsMaximized = false);

    private void HandleFocus() => this.Update(() => this.IsFocused = true);

    private void HandleBlur() => this.Update(() => this.IsFocused = false);

    private void HandleClosed()
    {
        lock (this.sync)
        {
            this.DetachCore();
        }

        events.NotifyWindowStateChanged();
    }

    /// <summary>
    /// Unsubscribes from the current window (if any). Must be called while holding <see cref="sync"/>.
    /// </summary>
    private void DetachCore()
    {
        if (this.window is not { } previous)
        {
            return;
        }

        previous.OnMaximize -= this.HandleMaximize;
        previous.OnUnmaximize -= this.HandleUnmaximize;
        previous.OnFocus -= this.HandleFocus;
        previous.OnBlur -= this.HandleBlur;
        previous.OnClosed -= this.HandleClosed;

        this.window = null;
    }

    private void Update(Action change)
    {
        change();
        events.NotifyWindowStateChanged();
    }
}
