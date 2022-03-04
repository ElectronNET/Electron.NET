using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API.Hubs
{
    public partial class HubElectron : Hub
    {
        public void ScreenOnDisplayAdded(string id, JObject display)
        {
            Electron.Screen.TriggerOnDisplayAdded(display.ToObject<Display>());
        }

        public void ScreenOnDisplayRemoved(string id, JObject display)
        {
            Electron.Screen.TriggerOnDisplayRemoved(display.ToObject<Display>());
        }

        public void ScreenOnDisplayMetricsChanged(string id, JArray args)
        {
            var display = ((JArray)args).First.ToObject<Display>();
            var metrics = ((JArray)args).Last.ToObject<string[]>();
            Electron.Screen.TriggerOnDisplayMetricsChanged(display, metrics);
        }
    }
}
