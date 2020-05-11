using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Add icons and context menus to the system's notification area.
    /// </summary>
    public sealed class PowerMonitor
    {
           


        /// <summary>
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
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
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
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
