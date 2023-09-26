namespace ElectronNET.API;

/// <summary>
/// The Electron.NET API
/// </summary>
public static class Electron
{
    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public static IpcMain IpcMain => IpcMain.Instance;

    /// <summary>
    /// Control your application's event lifecycle.
    /// </summary>
    public static App App => App.Instance;

    /// <summary>
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public static AutoUpdater AutoUpdater => AutoUpdater.Instance;

    /// <summary>
    /// Control your windows.
    /// </summary>
    public static WindowManager WindowManager => WindowManager.Instance;

    /// <summary>
    /// Create native application menus and context menus.
    /// </summary>
    public static Menu Menu => Menu.Instance;

    /// <summary>
    /// Display native system dialogs for opening and saving files, alerting, etc.
    /// </summary>
    public static Dialog Dialog => Dialog.Instance;

    /// <summary>
    /// Create OS desktop notifications
    /// </summary>
    public static Notification Notification => Notification.Instance;

    /// <summary>
    /// Add icons and context menus to the system’s notification area.
    /// </summary>
    public static Tray Tray => Tray.Instance;

    /// <summary>
    /// Detect keyboard events when the application does not have keyboard focus.
    /// </summary>
    public static GlobalShortcut GlobalShortcut => GlobalShortcut.Instance;

    /// <summary>
    /// Manage files and URLs using their default applications.
    /// </summary>
    public static Shell Shell => Shell.Instance;

    /// <summary>
    /// Retrieve information about screen size, displays, cursor position, etc.
    /// </summary>
    public static Screen Screen => Screen.Instance;

    /// <summary>
    /// Perform copy and paste operations on the system clipboard.
    /// </summary>
    public static Clipboard Clipboard => Clipboard.Instance;

    /// <summary>
    /// Allows you to execute native JavaScript/TypeScript code from the host process.
    /// 
    /// It is only possible if the Electron.NET CLI has previously added an 
    /// ElectronHostHook directory:
    /// <c>electronize add HostHook</c>
    /// </summary>
    public static HostHook HostHook => HostHook.Instance;

    /// <summary>
    /// Allows you to execute native Lock and Unlock process.       
    /// </summary>
    public static PowerMonitor PowerMonitor => PowerMonitor.Instance;

    /// <summary>
    /// Read and respond to changes in Chromium's native color theme.
    /// </summary>
    public static NativeTheme NativeTheme => NativeTheme.Instance;

    /// <summary>
    /// Control your app in the macOS dock.
    /// </summary>
    public static Dock Dock => Dock.Instance;
}