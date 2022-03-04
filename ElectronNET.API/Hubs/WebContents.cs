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
        public void WebContentOnCrashed(int id, bool crashed)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.WebContents.TriggerOnCrashed(crashed);
        }

        public void WebContentOnDidFinishLoad(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.WebContents.TriggerOnDidFinishLoad();
        }
    }
}
