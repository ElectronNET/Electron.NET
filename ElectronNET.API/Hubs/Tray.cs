using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ElectronNET.API.Hubs
{
    public partial class HubElectron : Hub
    {
        public void TrayOnClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnClick(trayClickEventArgs, bounds);
        }

        public void TrayOnRightClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnRightClick(trayClickEventArgs, bounds);
        }

        public void TrayOnDoubleClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnRightClick(trayClickEventArgs, bounds);
        }

        public void TrayOnBalloonShow(string id)
        {
            Tray.Instance.TriggerOnBalloonShow();
        }

        public void TrayOnBalloonClick(string id)
        {
            Tray.Instance.TriggerOnBalloonClick();
        }

        public void TrayOnBalloonClosed(string id)
        {
            Tray.Instance.TriggerOnBalloonClosed();
        }

        public void TrayOnMenuItemClicked(string id)
        {
            MenuItem menuItem = Tray.Instance.MenuItems.Where(x => x.Id == id).FirstOrDefault();
            menuItem?.Click();
        }
    }
}
