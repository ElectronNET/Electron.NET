using System;
using System.Threading.Tasks;

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
            add
            {
                if (_lockScreen == null)
                {
                    BridgeConnector.Socket.On("pm-lock-screen" , () =>
                    {
                        _lockScreen();
                    });

                    BridgeConnector.Socket.Emit("register-pm-lock-screen");
                }
                _lockScreen += value;
            }
            remove
            {
                _lockScreen -= value;

                if (_lockScreen == null)
                    BridgeConnector.Socket.Off("pm-lock-screen");
            }
        }

        private event Action _lockScreen;

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        public event Action OnUnLockScreen
        {
            add
            {
                if (_unlockScreen == null)
                {
                    BridgeConnector.Socket.On("pm-unlock-screen", () =>
                    {
                        _unlockScreen();
                    });

                    BridgeConnector.Socket.Emit("register-pm-unlock-screen");
                }
                _unlockScreen += value;
            }
            remove
            {
                _unlockScreen -= value;

                if (_unlockScreen == null)
                    BridgeConnector.Socket.Off("pm-unlock-screen");
            }
        }

        private event Action _unlockScreen;

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        public event Action OnSuspend
        {
            add
            {
                if (_suspend == null)
                {
                    BridgeConnector.Socket.On("pm-suspend", () =>
                    {
                        _suspend();
                    });

                    BridgeConnector.Socket.Emit("register-pm-suspend");
                }
                _suspend += value;
            }
            remove
            {
                _suspend -= value;

                if (_suspend == null)
                    BridgeConnector.Socket.Off("pm-suspend");
            }
        }

        private event Action _suspend;

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        public event Action OnResume
        {
            add
            {
                if (_resume == null)
                {
                    BridgeConnector.Socket.On("pm-resume", () =>
                    {
                        _resume();
                    });

                    BridgeConnector.Socket.Emit("register-pm-resume");
                }
                _resume += value;
            }
            remove
            {
                _resume -= value;

                if (_resume == null)
                    BridgeConnector.Socket.Off("pm-resume");
            }
        }

        private event Action _resume;

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        public event Action OnAC
        {
            add
            {
                if (_onAC == null)
                {
                    BridgeConnector.Socket.On("pm-on-ac", () =>
                    {
                        _onAC();
                    });

                    BridgeConnector.Socket.Emit("register-pm-on-ac");
                }
                _onAC += value;
            }
            remove
            {
                _onAC -= value;

                if (_onAC == null)
                    BridgeConnector.Socket.Off("pm-on-ac");
            }
        }

        private event Action _onAC;

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        public event Action OnBattery
        {
            add
            {
                if (_onBattery == null)
                {
                    BridgeConnector.Socket.On("pm-on-battery", () =>
                    {
                        _onBattery();
                    });

                    BridgeConnector.Socket.Emit("register-pm-on-battery");
                }
                _onBattery += value;
            }
            remove
            {
                _onBattery -= value;

                if (_onBattery == null)
                    BridgeConnector.Socket.Off("pm-on-battery");
            }
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
            add
            {
                if (_shutdown == null)
                {
                    BridgeConnector.Socket.On("pm-shutdown", () =>
                    {
                        _shutdown();
                    });

                    BridgeConnector.Socket.Emit("register-pm-shutdown");
                }
                _shutdown += value;
            }
            remove
            {
                _shutdown -= value;

                if (_shutdown == null)
                    BridgeConnector.Socket.Off("pm-on-shutdown");
            }
        }

        private event Action _shutdown;

        private static PowerMonitor _powerMonitor;
        private static object _syncRoot = new object();

        internal PowerMonitor() { }

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
