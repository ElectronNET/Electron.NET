using Microsoft.Extensions.Logging;
using System.Runtime.Versioning;
using System;
using System.Collections.Generic;

namespace ElectronNET.API
{
    /// <summary>
    /// The Electron.NET API
    /// </summary>
    public static partial class Electron
    {
        private static ILoggerFactory loggerFactory;

        /// <summary>
        /// Reads the auth key from the command line. This method must be called first thing.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void ReadAuth()
        {
            if (!string.IsNullOrEmpty(BridgeConnector.AuthKey))
            {
                throw new Exception($"Don't call ReadAuth twice or from with {nameof(Experimental)}.{nameof(Experimental.StartElectronForDevelopment)}");
            }

            var line = Console.ReadLine();

            if(line.StartsWith("Auth="))
            {
                BridgeConnector.AuthKey = line.Substring("Auth=".Length);
            }
            else
            {
                throw new Exception("The call to Electron.ReadAuth must be the first thing your app entry point does");
            }
        }

        /// <summary>
        /// Sets the logger factory to be used by Electron, if any
        /// </summary>
        public static ILoggerFactory LoggerFactory
        {
            private get => loggerFactory; set
            {
                loggerFactory = value;
                BridgeConnector.Logger = value.CreateLogger<App>();
            }
        }
        /// <summary>
        /// Communicate asynchronously from the main process to renderer processes.
        /// </summary>
        public static IpcMain IpcMain { get { return IpcMain.Instance; } }

        /// <summary>
        /// Control your application's event lifecycle.
        /// </summary>
        public static App App { get { return App.Instance; } }

        /// <summary>
        /// Enable apps to automatically update themselves. Based on electron-updater.
        /// </summary>
        public static AutoUpdater AutoUpdater { get { return AutoUpdater.Instance; } }

        /// <summary>
        /// Control your windows.
        /// </summary>
        public static WindowManager WindowManager { get { return WindowManager.Instance; } }

        /// <summary>
        /// Create native application menus and context menus.
        /// </summary>
        public static Menu Menu { get { return Menu.Instance; } }

        /// <summary>
        /// Display native system dialogs for opening and saving files, alerting, etc.
        /// </summary>
        public static Dialog Dialog { get { return Dialog.Instance; } }

        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        public static Notification Notification { get { return Notification.Instance; } }

        /// <summary>
        /// Add icons and context menus to the system’s notification area.
        /// </summary>
        public static Tray Tray { get { return Tray.Instance; } }

        /// <summary>
        /// Detect keyboard events when the application does not have keyboard focus.
        /// </summary>
        public static GlobalShortcut GlobalShortcut { get { return GlobalShortcut.Instance; } }

        /// <summary>
        /// Manage files and URLs using their default applications.
        /// </summary>
        public static Shell Shell { get { return Shell.Instance; } }

        /// <summary>
        /// Retrieve information about screen size, displays, cursor position, etc.
        /// </summary>
        public static Screen Screen { get { return Screen.Instance; } }

        /// <summary>
        /// Access information about media sources that can be used to capture audio and video from the desktop using the navigator.mediaDevices.getUserMedia API.
        /// </summary>
        public static DesktopCapturer DesktopCapturer { get { return DesktopCapturer.Instance; } }

        /// <summary>
        /// Perform copy and paste operations on the system clipboard.
        /// </summary>
        public static Clipboard Clipboard { get { return Clipboard.Instance; } }

        /// <summary>
        /// Allows you to execute native JavaScript/TypeScript code from the host process.
        /// 
        /// It is only possible if the Electron.NET CLI has previously added an 
        /// ElectronHostHook directory:
        /// <c>electronize add HostHook</c>
        /// </summary>
        public static HostHook HostHook { get { return HostHook.Instance; } }

        /// <summary>
        /// Allows you to execute native Lock and Unlock process.       
        /// </summary>
        public static PowerMonitor PowerMonitor { get { return PowerMonitor.Instance; } }

        /// <summary>
        /// Read and respond to changes in Chromium's native color theme.
        /// </summary>
        public static NativeTheme NativeTheme { get { return NativeTheme.Instance; } }

        /// <summary>
        /// Control your app in the macOS dock.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public static Dock Dock { get { return Dock.Instance; } }
    }
}