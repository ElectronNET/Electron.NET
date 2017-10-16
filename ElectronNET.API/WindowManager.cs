using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public sealed class WindowManager
    {
        private static WindowManager _windowManager;

        internal WindowManager() { }

        internal static WindowManager Instance
        {
            get
            {
                if (_windowManager == null)
                {
                    _windowManager = new WindowManager();
                }

                return _windowManager;
            }
        }

        public IReadOnlyCollection<BrowserWindow> BrowserWindows { get { return _browserWindows.AsReadOnly(); } }
        private List<BrowserWindow> _browserWindows = new List<BrowserWindow>();

        public async Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost")
        {
            return await CreateWindowAsync(new BrowserWindowOptions(), loadUrl);
        }

        public Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost")
        {
            var taskCompletionSource = new TaskCompletionSource<BrowserWindow>();

            BridgeConnector.Socket.On("BrowserWindowCreated", (id) =>
            {
                BridgeConnector.Socket.Off("BrowserWindowCreated");

                string windowId = id.ToString();
                BrowserWindow browserWindow = new BrowserWindow(int.Parse(windowId));
                _browserWindows.Add(browserWindow);

                taskCompletionSource.SetResult(browserWindow);
            });

            BridgeConnector.Socket.Off("BrowserWindowClosed");
            BridgeConnector.Socket.On("BrowserWindowClosed", (ids) =>
            {
                var browserWindowIds = ((JArray)ids).ToObject<int[]>();

                for (int index = 0; index < _browserWindows.Count; index++)
                {
                    if (!browserWindowIds.Contains(_browserWindows[index].Id))
                    {
                        _browserWindows.RemoveAt(index);
                    }
                }
            });

            if (loadUrl.ToUpper() == "HTTP://LOCALHOST")
            {
                loadUrl = $"{loadUrl}:{BridgeSettings.WebPort}";
            }

            BridgeConnector.Socket.Emit("createBrowserWindow", JObject.FromObject(options, _jsonSerializer), loadUrl);

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
