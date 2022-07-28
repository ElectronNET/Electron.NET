using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Monitor power state changes..
    /// </summary>
    public sealed class PowerMonitor : IPowerMonitor
    {
        /// <summary>
        /// Emitted when the system is about to lock the screen. 
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnLockScreen
        {
            add
            {
                if (_lockScreen == null)
                {
                    BridgeConnector.On("pm-lock-screen" , () =>
                    {
                        _lockScreen();
                    });

                    BridgeConnector.Emit("register-pm-lock-screen");
                }
                _lockScreen += value;
            }
            remove
            {
                _lockScreen -= value;

                if (_lockScreen == null)
                    BridgeConnector.Off("pm-lock-screen");
            }
        }

        private event Action _lockScreen;

        /// <summary>
        /// Emitted when the system is about to unlock the screen. 
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnUnLockScreen
        {
            add
            {
                if (_unlockScreen == null)
                {
                    BridgeConnector.On("pm-unlock-screen", () =>
                    {
                        _unlockScreen();
                    });

                    BridgeConnector.Emit("register-pm-unlock-screen");
                }
                _unlockScreen += value;
            }
            remove
            {
                _unlockScreen -= value;

                if (_unlockScreen == null)
                    BridgeConnector.Off("pm-unlock-screen");
            }
        }

        private event Action _unlockScreen;

        /// <summary>
        /// Emitted when the system is suspending.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnSuspend
        {
            add
            {
                if (_suspend == null)
                {
                    BridgeConnector.On("pm-suspend", () =>
                    {
                        _suspend();
                    });

                    BridgeConnector.Emit("register-pm-suspend");
                }
                _suspend += value;
            }
            remove
            {
                _suspend -= value;

                if (_suspend == null)
                    BridgeConnector.Off("pm-suspend");
            }
        }

        private event Action _suspend;

        /// <summary>
        /// Emitted when system is resuming.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnResume
        {
            add
            {
                if (_resume == null)
                {
                    BridgeConnector.On("pm-resume", () =>
                    {
                        _resume();
                    });

                    BridgeConnector.Emit("register-pm-resume");
                }
                _resume += value;
            }
            remove
            {
                _resume -= value;

                if (_resume == null)
                    BridgeConnector.Off("pm-resume");
            }
        }

        private event Action _resume;

        /// <summary>
        /// Emitted when the system changes to AC power.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnAC
        {
            add
            {
                if (_onAC == null)
                {
                    BridgeConnector.On("pm-on-ac", () =>
                    {
                        _onAC();
                    });

                    BridgeConnector.Emit("register-pm-on-ac");
                }
                _onAC += value;
            }
            remove
            {
                _onAC -= value;

                if (_onAC == null)
                    BridgeConnector.Off("pm-on-ac");
            }
        }

        private event Action _onAC;

        /// <summary>
        /// Emitted when system changes to battery power.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnBattery
        {
            add
            {
                if (_onBattery == null)
                {
                    BridgeConnector.On("pm-on-battery", () =>
                    {
                        _onBattery();
                    });

                    BridgeConnector.Emit("register-pm-on-battery");
                }
                _onBattery += value;
            }
            remove
            {
                _onBattery -= value;

                if (_onBattery == null)
                    BridgeConnector.Off("pm-on-battery");
            }
        }

        private event Action _onBattery;


        /// <summary>
        /// Emitted when the system is about to reboot or shut down. If the event handler
        /// invokes `e.preventDefault()`, Electron will attempt to delay system shutdown in
        /// order for the app to exit cleanly.If `e.preventDefault()` is called, the app
        /// should exit as soon as possible by calling something like `app.quit()`.
        /// </summary>
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]

        public event Action OnShutdown
        {
            add
            {
                if (_shutdown == null)
                {
                    BridgeConnector.On("pm-shutdown", () =>
                    {
                        _shutdown();
                    });

                    BridgeConnector.Emit("register-pm-shutdown");
                }
                _shutdown += value;
            }
            remove
            {
                _shutdown -= value;

                if (_shutdown == null)
                    BridgeConnector.Off("pm-on-shutdown");
            }
        }

        private event Action _shutdown;

        private static PowerMonitor _powerMonitor;
        private static readonly object _syncRoot = new();

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
