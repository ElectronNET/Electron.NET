using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Create OS desktop notifications
    /// </summary>
    public sealed class Notification : INotification
    {
        private static Notification _notification;
        private static readonly object _syncRoot = new();

        internal Notification() { }

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

        private static readonly List<NotificationOptions> _notificationOptions = new();

        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        /// <param name="notificationOptions"></param>
        public void Show(NotificationOptions notificationOptions)
        {
            GenerateIDsForDefinedActions(notificationOptions);

            BridgeConnector.Emit("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }

        private static void GenerateIDsForDefinedActions(NotificationOptions notificationOptions)
        {
            bool isActionDefined = false;

            if (notificationOptions.OnShow != null)
            {
                notificationOptions.ShowID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Off("NotificationEventShow");
                BridgeConnector.On<string>("NotificationEventShow", (id) => {
                    _notificationOptions.Single(x => x.ShowID == id).OnShow();
                });
            }

            if (notificationOptions.OnClick != null)
            {
                notificationOptions.ClickID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Off("NotificationEventClick");
                BridgeConnector.On<string>("NotificationEventClick", (id) => {
                    _notificationOptions.Single(x => x.ClickID == id).OnClick();
                });
            }

            if (notificationOptions.OnClose != null)
            {
                notificationOptions.CloseID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Off("NotificationEventClose");
                BridgeConnector.On<string>("NotificationEventClose", (id) => {
                    _notificationOptions.Single(x => x.CloseID == id.ToString()).OnClose();
                });
            }

            if (notificationOptions.OnReply != null)
            {
                notificationOptions.ReplyID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Off("NotificationEventReply");
                BridgeConnector.On<string[]>("NotificationEventReply", (args) => {
                    _notificationOptions.Single(x => x.ReplyID == args[0].ToString()).OnReply(args[1].ToString());
                });
            }

            if (notificationOptions.OnAction != null)
            {
                notificationOptions.ActionID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Off("NotificationEventAction");
                BridgeConnector.On<string[]>("NotificationEventAction", (args) => {
                    _notificationOptions.Single(x => x.ReplyID == args[0].ToString()).OnAction(args[1].ToString());
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
            var taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<bool>("notificationIsSupportedComplete", (isSupported) =>
            {
                BridgeConnector.Off("notificationIsSupportedComplete");
                taskCompletionSource.SetResult(isSupported);
            });

            BridgeConnector.Emit("notificationIsSupported");

            return taskCompletionSource.Task;
        }

        private static readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
