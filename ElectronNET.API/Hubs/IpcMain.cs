using ElectronNET.API.Models;
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
        public void IpcOnChannel(string channel, JArray args)
        {
            foreach (var item in Electron.SignalrObservedJArray.Where(x => x.Channel == channel).ToList())
            {
                Electron.SignalrObservedJArray.Remove(item);
            }

            SignalrResponse signalrResponse = new SignalrResponse();
            signalrResponse.Channel = channel;
            signalrResponse.Value = args;
            Electron.SignalrObservedJArray.Add(signalrResponse);
        }

        public void IpcMainChannelWithId(string channel, JObject args)
        {
            foreach (var item in Electron.SignalrObservedJObject.Where(x => x.Channel == channel).ToList())
            {
                Electron.SignalrObservedJObject.Remove(item);
            }

            SignalrResponseJObject signalrResponse = new SignalrResponseJObject();
            signalrResponse.Channel = channel;
            signalrResponse.Value = args;
            Electron.SignalrObservedJObject.Add(signalrResponse);
        }
    }
}
