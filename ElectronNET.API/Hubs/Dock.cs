using ElectronNET.API.Entities;
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
        public void DockMenuItemClicked(string id)
        {
            MenuItem menuItem = Dock.Instance.MenuItems.Where(x => x.Id == id).FirstOrDefault();
            menuItem?.Click();
        }
    }
}
