using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ElectronNET.API
{
    public sealed class Notification
    {
        private static Notification _notification;

        internal Notification() { }

        internal static Notification Instance
        {
            get
            {
                if (_notification == null)
                {
                    _notification = new Notification();
                }

                return _notification;
            }
        }

        public void Show(NotificationOptions notificationOptions)
        {
            BridgeConnector.Socket.Emit("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
