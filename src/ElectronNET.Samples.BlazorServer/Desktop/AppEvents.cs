namespace ElectronNET.Samples.BlazorServer.Desktop;

/// <summary>
/// Carries events from the Electron side (menus, window events, ...) to the Blazor UI.
/// </summary>
/// <remarks>
/// Electron callbacks (e.g., a menu click) run outside of any Blazor circuit. They cannot
/// use a component's NavigationManager or call StateHasChanged directly. Instead, they raise
/// an event on this singleton, and the components that are interested subscribe to it and
/// marshal back onto their renderer with <c>InvokeAsync</c>.
/// </remarks>
public sealed class AppEvents
{
    /// <summary>
    /// Raised when something outside the UI (e.g., the application menu) wants to navigate.
    /// </summary>
    public event Action<string>? NavigationRequested;

    /// <summary>
    /// Raised when the state of the main window changed (maximized, focused, ...).
    /// </summary>
    public event Action? WindowStateChanged;

    public void RequestNavigation(string uri) => NavigationRequested?.Invoke(uri);

    public void NotifyWindowStateChanged() => WindowStateChanged?.Invoke();
}
