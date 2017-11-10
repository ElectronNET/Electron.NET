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
    /// 
    /// </summary>
    public sealed class WindowManager
    {
        private static WindowManager _windowManager;
        private static object _syncRoot = new Object();

        internal WindowManager() { }

        internal static WindowManager Instance
        {
            get
            {
                if (_windowManager == null)
                {
                    lock (_syncRoot)
                    {
                        if (_windowManager == null)
                        {
                            _windowManager = new WindowManager();
                        }
                    }
                }

                return _windowManager;
            }
        }

        /// <summary>
        /// Quit when all windows are closed. (Default is true)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [quit window all closed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsQuitOnWindowAllClosed
        {
            get { return _isQuitOnWindowAllClosed; }
            set
            {
                BridgeConnector.Socket.Emit("quit-app-window-all-closed-event", value);
                _isQuitOnWindowAllClosed = value;
            }
        }
        private bool _isQuitOnWindowAllClosed = true;

        /// <summary>
        /// Gets the browser windows.
        /// </summary>
        /// <value>
        /// The browser windows.
        /// </value>
        public IReadOnlyCollection<BrowserWindow> BrowserWindows { get { return _browserWindows.AsReadOnly(); } }
        private List<BrowserWindow> _browserWindows = new List<BrowserWindow>();

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        public async Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost")
        {
            return await CreateWindowAsync(new BrowserWindowOptions(), loadUrl);
        }

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
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
