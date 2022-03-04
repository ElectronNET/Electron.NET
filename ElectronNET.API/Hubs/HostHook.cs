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
        public void HostHookResult(string socketEventName, params dynamic[] arguments)
        {

        }

        public void HostHookError(string socketEventName, params dynamic[] arguments)
        {

        }
    }
}
