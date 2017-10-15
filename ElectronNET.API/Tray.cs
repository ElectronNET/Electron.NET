using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ElectronNET.API
{
    public sealed class Tray
    {
        private static Tray _tray;

        internal Tray() { }

        internal static Tray Instance
        {
            get
            {
                if (_tray == null)
                {
                    _tray = new Tray();
                }

                return _tray;
            }
        }

        public void Show(string image, MenuItem[] menuItems)
        {
            BridgeConnector.Socket.Emit("create-tray", image, JArray.FromObject(menuItems, _jsonSerializer));
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
