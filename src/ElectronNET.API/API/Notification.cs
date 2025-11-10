using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Create OS desktop notifications
    /// </summary>
    public sealed class Notification
    {
        private static Notification _notification;
        private static object _syncRoot = new object();

        internal Notification()
        {
        }

        internal static Notification Instance
        {
            get
            {
                if (_notification == null)
                {
                    lock (_syncRoot)
                    {
                        if (_notification == null)
                        {
                            _notification = new Notification();
                        }
                    }
                }

                return _notification;
            }
        }

        private static List<NotificationOptions> _notificationOptions = new List<NotificationOptions>();

        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        /// <param name="notificationOptions"></param>
        public void Show(NotificationOptions notificationOptions)
        {
            GenerateIDsForDefinedActions(notificationOptions);

            BridgeConnector.Socket.Emit("createNotification", notificationOptions);
        }

        private static void GenerateIDsForDefinedActions(NotificationOptions notificationOptions)
        {
            bool isActionDefined = false;

            if (notificationOptions.OnShow != null)
            {
                notificationOptions.ShowID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventShow");
                BridgeConnector.Socket.On<string>("NotificationEventShow", (id) => { _notificationOptions.Single(x => x.ShowID == id).OnShow(); });
            }

            if (notificationOptions.OnClick != null)
            {
                notificationOptions.ClickID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClick");
                BridgeConnector.Socket.On<string>("NotificationEventClick", (id) => { _notificationOptions.Single(x => x.ClickID == id).OnClick(); });
            }

            if (notificationOptions.OnClose != null)
            {
                notificationOptions.CloseID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClose");
                BridgeConnector.Socket.On<string>("NotificationEventClose", (id) => { _notificationOptions.Single(x => x.CloseID == id).OnClose(); });
            }

            if (notificationOptions.OnReply != null)
            {
                notificationOptions.ReplyID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventReply");
                BridgeConnector.Socket.On<string[]>("NotificationEventReply", (args) =>
                {
                    _notificationOptions.Single(x => x.ReplyID == args[0]).OnReply(args[1]);
                });
            }

            if (notificationOptions.OnAction != null)
            {
                notificationOptions.ActionID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventAction");
                BridgeConnector.Socket.On<string[]>("NotificationEventAction", (args) =>
                {
                    _notificationOptions.Single(x => x.ActionID == args[0]).OnAction(args[1]);
                });
            }

            if (isActionDefined)
            {
                _notificationOptions.Add(notificationOptions);
            }
        }

        /// <summary>
        /// Whether or not desktop notifications are supported on the current system.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsSupportedAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.Once<bool>("notificationIsSupportedComplete", tcs.SetResult);
            BridgeConnector.Socket.Emit("notificationIsSupported");

            return tcs.Task;
        }


    }
}
