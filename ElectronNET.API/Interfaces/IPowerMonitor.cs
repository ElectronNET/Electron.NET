using System;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Monitor power state changes..
    /// </summary>
    public interface IPowerMonitor
    {
        /// <summary>
        /// Emitted when the system is about to lock the screen. 
        /// </summary>
        event Action OnLockScreen;

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        event Action OnUnLockScreen;

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        event Action OnSuspend;

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        event Action OnResume;

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        event Action OnAC;

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        event Action OnBattery;

        /// <summary>
        /// Emitted when the system is about to reboot or shut down. If the event handler
        /// invokes `e.preventDefault()`, Electron will attempt to delay system shutdown in
        /// order for the app to exit cleanly.If `e.preventDefault()` is called, the app
        /// should exit as soon as possible by calling something like `app.quit()`.
        /// </summary>
        event Action OnShutdown;
    }
}