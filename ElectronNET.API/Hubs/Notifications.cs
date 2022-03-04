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
        public void NotificationEventOnShow(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnShow();
        }

        public void NotificationEventOnClick(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnClick();
        }

        public void NotificationEventOnClose(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnClose();
        }

        public void NotificationEventOnReply(string id, JArray args)
        {
            var arguments = ((JArray)args).ToObject<string[]>();
            Notification.Instance.NotificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnReply(arguments[1].ToString());
        }

        public void NotificationEventOnAction(string id, JArray args)
        {
            var arguments = ((JArray)args).ToObject<string[]>();
            Notification.Instance.NotificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnAction(arguments[1].ToString());
        }
    }
}
