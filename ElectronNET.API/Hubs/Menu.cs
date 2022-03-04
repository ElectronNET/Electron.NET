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
        public void MenuMenuItemClicked(string id)
        {
            MenuItem menuItem = Menu.Instance.GetMenuItem(id.ToString());
            menuItem.Click?.Invoke();
        }

        public void MenuContextMenuItemClicked(string id, JArray results)
        {
            var itemId = ((JArray)results).First.ToString();
            var browserWindowId = (int)((JArray)results).Last;

            MenuItem menuItem = Menu.Instance.ContextMenuItems[browserWindowId].Where(x => x.Id == itemId).FirstOrDefault();
            menuItem.Click?.Invoke();
        }
    }
}
