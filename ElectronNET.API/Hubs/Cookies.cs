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
        public void CookiesOnChanged(int id, JArray jarray)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.WebContents.Session.Cookies.TriggerOnChanged(jarray);
        }
    }
}
