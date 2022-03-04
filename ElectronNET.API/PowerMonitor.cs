using Microsoft.AspNetCore.SignalR;
using System;
using System.Runtime.Versioning;
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
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public event Action OnLockScreen
        {
            add
            {
                if (_lockScreen == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-lock-screen");
                }
                _lockScreen += value;
            }
            remove
            {
                _lockScreen -= value;
            }
        }

        public void TriggerOnLockScreen()
        {
            _lockScreen();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-unlock-screen");
                }
                _unlockScreen += value;
            }
            remove
            {
                _unlockScreen -= value;
            }
        }

        public void TriggerOnUnLockScreen()
        {
            _unlockScreen();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-suspend");
                }
                _suspend += value;
            }
            remove
            {
                _suspend -= value;
            }
        }

        public void TriggerOnSuspend()
        {
            _suspend();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-resume");
                }
                _resume += value;
            }
            remove
            {
                _resume -= value;
            }
        }

        public void TriggerOnResume()
        {
            _resume();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-on-ac");
                }
                _onAC += value;
            }
            remove
            {
                _onAC -= value;
            }
        }

        public void TriggerOnAC()
        {
            _onAC();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-on-battery");
                }
                _onBattery += value;
            }
            remove
            {
                _onBattery -= value;
            }
        }

        public void TriggerOnBattery()
        {
            _onBattery();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-pm-shutdown");
                }
                _shutdown += value;
            }
            remove
            {
                _shutdown -= value;
            }
        }

        public void TriggerOnShutdown()
        {
            _shutdown();
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
