using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API.Hubs
{
    public partial class HubElectron : Hub
    {
        public void PowerMonitorOnLockScreen()
        {
            Electron.PowerMonitor.TriggerOnLockScreen();
        }

        public void PowerMonitorOnUnLockScreen()
        {
            Electron.PowerMonitor.TriggerOnUnLockScreen();
        }

        public void PowerMonitorOnSuspend()
        {
            Electron.PowerMonitor.TriggerOnSuspend();
        }

        public void PowerMonitorOnResume()
        {
            Electron.PowerMonitor.TriggerOnResume();
        }

        public void PowerMonitorOnAC()
        {
            Electron.PowerMonitor.TriggerOnAC();
        }

        public void PowerMonitorOnBattery()
        {
            Electron.PowerMonitor.TriggerOnBattery();
        }

        public void PowerMonitorOnShutdown()
        {
            Electron.PowerMonitor.TriggerOnShutdown();
        }
    }
}
