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
        public void BootstrapUpdateOpenIDsEvent(int[] id)
        {
            WindowManager.Instance.TriggerOnBootstrapUpdateOpenIDsEvent(id);
        }
    }
}
