namespace ElectronNET.API
{
    public static class Electron
    {
        /// <summary>
        /// Communicate asynchronously from the main process to renderer processes.
        /// </summary>
        public static IpcMain IpcMain { get { return IpcMain.Instance; } }

        /// <summary>
        /// Control your application's event lifecycle.
        /// </summary>
        public static App App { get { return App.Instance; } }

        /// <summary>
        /// Control your windows.
        /// </summary>
        public static WindowManager WindowManager { get { return WindowManager.Instance; } }

        /// <summary>
        /// Create native application menus and context menus.
        /// </summary>
        public static Menu Menu { get { return Menu.Instance; } }
    }
}
