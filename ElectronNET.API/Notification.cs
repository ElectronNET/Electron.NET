using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
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

        public IReadOnlyCollection<NotificationOptions> NotificationOptions { get { return _notificationOptions.AsReadOnly(); } }
        private static readonly List<NotificationOptions> _notificationOptions = new();

        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        /// <param name="notificationOptions"></param>
        public async void Show(NotificationOptions notificationOptions)
        {
            GenerateIDsForDefinedActions(notificationOptions);

            await Electron.SignalrElectron.Clients.All.SendAsync("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }

        private static void GenerateIDsForDefinedActions(NotificationOptions notificationOptions)
        {
            bool isActionDefined = false;

            if (notificationOptions.OnShow != null)
            {
                notificationOptions.ShowID = Guid.NewGuid().ToString();
                isActionDefined = true;
            }

            if (notificationOptions.OnClick != null)
            {
                notificationOptions.ClickID = Guid.NewGuid().ToString();
                isActionDefined = true;
            }

            if (notificationOptions.OnClose != null)
            {
                notificationOptions.CloseID = Guid.NewGuid().ToString();
                isActionDefined = true;
            }

            if (notificationOptions.OnReply != null)
            {
                notificationOptions.ReplyID = Guid.NewGuid().ToString();
                isActionDefined = true;
            }

            if (notificationOptions.OnAction != null)
            {
                notificationOptions.ActionID = Guid.NewGuid().ToString();
                isActionDefined = true;
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
        public async Task<bool> IsSupportedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("notificationIsSupported");
        }

        private static readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
