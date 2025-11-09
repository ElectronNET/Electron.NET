using ElectronNET.Common;
using System;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Monitor power state changes..
    /// </summary>
    public sealed class PowerMonitor
    {
        /// <summary>
        /// Emitted when the system is about to lock the screen. 
        /// </summary>
        public event Action OnLockScreen
        {
            add => ApiEventManager.AddEvent("pm-lock-screen", string.Empty, _lockScreen, value);
            remove => ApiEventManager.RemoveEvent("pm-lock-screen", string.Empty, _lockScreen, value);
        }

        private event Action _lockScreen;

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        public event Action OnUnLockScreen
        {
            add => ApiEventManager.AddEvent("pm-unlock-screen", string.Empty, _unlockScreen, value);
            remove => ApiEventManager.RemoveEvent("pm-unlock-screen", string.Empty, _unlockScreen, value);
        }

        private event Action _unlockScreen;

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        public event Action OnSuspend
        {
            add => ApiEventManager.AddEvent("pm-suspend", string.Empty, _suspend, value);
            remove => ApiEventManager.RemoveEvent("pm-suspend", string.Empty, _suspend, value);
        }

        private event Action _suspend;

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        public event Action OnResume
        {
            add => ApiEventManager.AddEvent("pm-resume", string.Empty, _resume, value);
            remove => ApiEventManager.RemoveEvent("pm-resume", string.Empty, _resume, value);
        }

        private event Action _resume;

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        public event Action OnAC
        {
            add => ApiEventManager.AddEvent("pm-on-ac", string.Empty, _onAC, value);
            remove => ApiEventManager.RemoveEvent("pm-on-ac", string.Empty, _onAC, value);
        }

        private event Action _onAC;

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        public event Action OnBattery
        {
            add => ApiEventManager.AddEvent("pm-on-battery", string.Empty, _onBattery, value);
            remove => ApiEventManager.RemoveEvent("pm-on-battery", string.Empty, _onBattery, value);
        }

        private event Action _onBattery;

        /// <summary>
        /// Emitted when the system is about to reboot or shut down. If the event handler
        /// invokes `e.preventDefault()`, Electron will attempt to delay system shutdown in
        /// order for the app to exit cleanly.If `e.preventDefault()` is called, the app
        /// should exit as soon as possible by calling something like `app.quit()`.
        /// </summary>
        public event Action OnShutdown
        {
            add => ApiEventManager.AddEvent("pm-shutdown", string.Empty, _shutdown, value);
            remove => ApiEventManager.RemoveEvent("pm-shutdown", string.Empty, _shutdown, value);
        }

        private event Action _shutdown;

        private static PowerMonitor _powerMonitor;
        private static object _syncRoot = new object();

        internal PowerMonitor()
        {
        }

        internal static PowerMonitor Instance
        {
            get
            {
                if (_powerMonitor == null)
                {
                    lock (_syncRoot)
                    {
                        if (_powerMonitor == null)
                        {
                            _powerMonitor = new PowerMonitor();
                        }
                    }
                }

                return _powerMonitor;
            }
        }
    }
}