using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Create OS desktop notifications
    /// </summary>
    public sealed class Notification
    {
        private static Notification _notification;
        private static object _syncRoot = new Object();

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

        private static List<NotificationOptions> _notificationOptions = new List<NotificationOptions>();

        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        /// <param name="notificationOptions"></param>
        public void Show(NotificationOptions notificationOptions)
        {
            GenerateIDsForDefinedActions(notificationOptions);

            BridgeConnector.Socket.Emit("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }

        private static void GenerateIDsForDefinedActions(NotificationOptions notificationOptions)
        {
            bool isActionDefined = false;

            if (notificationOptions.OnShow != null)
            {
                notificationOptions.ShowID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventShow");
                BridgeConnector.Socket.On("NotificationEventShow", (id) => {
                    _notificationOptions.Single(x => x.ShowID == id.ToString()).OnShow();
                });
            }

            if (notificationOptions.OnClick != null)
            {
                notificationOptions.ClickID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClick");
                BridgeConnector.Socket.On("NotificationEventClick", (id) => {
                    _notificationOptions.Single(x => x.ClickID == id.ToString()).OnClick();
                });
            }

            if (notificationOptions.OnClose != null)
            {
                notificationOptions.CloseID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventClose");
                BridgeConnector.Socket.On("NotificationEventClose", (id) => {
                    _notificationOptions.Single(x => x.CloseID == id.ToString()).OnClose();
                });
            }

            if (notificationOptions.OnReply != null)
            {
                notificationOptions.ReplyID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventReply");
                BridgeConnector.Socket.On("NotificationEventReply", (args) => {
                    var arguments = ((JArray)args).ToObject<string[]>();
                    _notificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnReply(arguments[1].ToString());
                });
            }

            if (notificationOptions.OnAction != null)
            {
                notificationOptions.ActionID = Guid.NewGuid().ToString();
                isActionDefined = true;

                BridgeConnector.Socket.Off("NotificationEventAction");
                BridgeConnector.Socket.On("NotificationEventAction", (args) => {
                    var arguments = ((JArray)args).ToObject<string[]>();
                    _notificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnAction(arguments[1].ToString());
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

            BridgeConnector.Socket.On("notificationIsSupportedComplete", (isSupported) =>
            {
                BridgeConnector.Socket.Off("notificationIsSupportedComplete");
                taskCompletionSource.SetResult((bool)isSupported);
            });

            BridgeConnector.Socket.Emit("notificationIsSupported");

            return taskCompletionSource.Task;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
