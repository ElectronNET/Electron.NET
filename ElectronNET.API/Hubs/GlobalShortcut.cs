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
        public void GlobalShortcutPressed(string id)
        {
            if (GlobalShortcut.Instance._shortcuts.ContainsKey(id.ToString()))
            {
                GlobalShortcut.Instance._shortcuts.Where(x => x.Key == id).FirstOrDefault().Value();
            }
        }
    }
}
