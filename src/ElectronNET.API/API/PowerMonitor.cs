using System;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Monitor power state changes..
    /// </summary>
    public sealed class PowerMonitor: ApiBase
    {
        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashedLower;

        /// <summary>
        /// Emitted when the system is about to lock the screen. 
        /// </summary>
        public event Action OnLockScreen
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        public event Action OnUnLockScreen
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        public event Action OnSuspend
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        public event Action OnResume
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        public event Action OnAC
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        public event Action OnBattery
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        /// <summary>
        /// Emitted when the system is about to reboot or shut down. If the event handler
        /// invokes `e.preventDefault()`, Electron will attempt to delay system shutdown in
        /// order for the app to exit cleanly.If `e.preventDefault()` is called, the app
        /// should exit as soon as possible by calling something like `app.quit()`.
        /// </summary>
        public event Action OnShutdown
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

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