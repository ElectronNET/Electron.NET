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
                BridgeConnector.Socket.On<JsonElement>("NotificationEventShow", (id) => { _notificationOptions.Single(x => x.ShowID == id.GetString()).OnShow(); });
            }

            if (notificationOptions.OnClick != null)
            {
                notificationOptions.ClickID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClick");
                BridgeConnector.Socket.On<JsonElement>("NotificationEventClick", (id) => { _notificationOptions.Single(x => x.ClickID == id.GetString()).OnClick(); });
            }

            if (notificationOptions.OnClose != null)
            {
                notificationOptions.CloseID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClose");
                BridgeConnector.Socket.On<JsonElement>("NotificationEventClose", (id) => { _notificationOptions.Single(x => x.CloseID == id.GetString()).OnClose(); });
            }

            if (notificationOptions.OnReply != null)
            {
                notificationOptions.ReplyID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventReply");
                BridgeConnector.Socket.On<JsonElement>("NotificationEventReply", (args) =>
                {
                    var arguments = args.Deserialize<string[]>(ElectronJson.Options);
                    _notificationOptions.Single(x => x.ReplyID == arguments[0]).OnReply(arguments[1]);
                });
            }

            if (notificationOptions.OnAction != null)
            {
                notificationOptions.ActionID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventAction");
                BridgeConnector.Socket.On<JsonElement>("NotificationEventAction", (args) =>
                {
                    var arguments = args.Deserialize<string[]>(ElectronJson.Options);
                    _notificationOptions.Single(x => x.ActionID == arguments[0]).OnAction(arguments[1]);
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
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On<JsonElement>("notificationIsSupportedComplete", (isSupported) =>
            {
                BridgeConnector.Socket.Off("notificationIsSupportedComplete");
                taskCompletionSource.SetResult(isSupported.GetBoolean());
            });

            BridgeConnector.Socket.Emit("notificationIsSupported");

            return taskCompletionSource.Task;
        }


    }
}
