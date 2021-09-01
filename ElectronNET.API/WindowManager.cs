using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WindowManager
    {
        private static WindowManager _windowManager;
        private static object _syncRoot = new object();

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
                BridgeConnector.Emit("quit-app-window-all-closed-event", value);
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
        public IReadOnlyCollection<BrowserWindow> BrowserWindows { get { return _browserWindows.Values.ToList().AsReadOnly(); } }

        private ConcurrentDictionary<int, BrowserWindow> _browserWindows = new ();

        /// <summary>
        /// Gets the browser views.
        /// </summary>
        /// <value>
        /// The browser view.
        /// </value>
        public IReadOnlyCollection<BrowserView> BrowserViews { get { return _browserViews.Values.ToList().AsReadOnly(); } }
        private ConcurrentDictionary<int, BrowserView> _browserViews = new ();

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
            var taskCompletionSource = new TaskCompletionSource<BrowserWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<int>("BrowserWindowCreated", (id) =>
            {
                BridgeConnector.Off("BrowserWindowCreated");

                var browserWindow = new BrowserWindow(id);
                
                _browserWindows[id] = browserWindow;

                taskCompletionSource.SetResult(browserWindow);
            });

            BridgeConnector.Off("BrowserWindowClosed");
            BridgeConnector.On<int[]>("BrowserWindowClosed", (browserWindowIds) =>
            {
                foreach(var id in browserWindowIds)
                {
                    _browserWindows.TryRemove(id, out _);
                }
            });

            if (string.Equals(loadUrl, "HTTP://LOCALHOST", StringComparison.InvariantCultureIgnoreCase))
            {
                loadUrl = $"{loadUrl}:{BridgeSettings.WebPort}";
            }

            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (IsWindows10())
            {
                options.Width = options.Width + 14;
                options.Height = options.Height + 7;
            }

            if (options.X == -1 && options.Y == -1)
            {
                options.X = 0;
                options.Y = 0;

                BridgeConnector.Emit("createBrowserWindow", JObject.FromObject(options, _jsonSerializer), loadUrl);
            }
            else
            {
                // Workaround Windows 10 / Electron Bug
                // https://github.com/electron/electron/issues/4045
                if (IsWindows10())
                {
                    options.X = options.X - 7;
                }

                BridgeConnector.Emit("createBrowserWindow", JObject.FromObject(options, _keepDefaultValuesSerializer), loadUrl);
            }

            return taskCompletionSource.Task;
        }

        private bool IsWindows10()
        {
            return OperatingSystem.IsWindowsVersionAtLeast(10);
        }

        /// <summary>
        /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
        /// It is like a child window, except that it is positioned relative to its owning window. 
        /// It is meant to be an alternative to the webview tag.
        /// </summary>
        /// <returns></returns>
        public Task<BrowserView> CreateBrowserViewAsync()
        {
            return CreateBrowserViewAsync(new BrowserViewConstructorOptions());
        }

        /// <summary>
        /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
        /// It is like a child window, except that it is positioned relative to its owning window. 
        /// It is meant to be an alternative to the webview tag.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<BrowserView> CreateBrowserViewAsync(BrowserViewConstructorOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<BrowserView>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<int>("BrowserViewCreated", (id) =>
            {
                BridgeConnector.Off("BrowserViewCreated");

                BrowserView browserView = new BrowserView(id);

                _browserViews[id] = browserView;

                taskCompletionSource.SetResult(browserView);
            });

            BridgeConnector.Emit("createBrowserView", JObject.FromObject(options, _keepDefaultValuesSerializer));

            return taskCompletionSource.Task;
        }

        private static JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        private static JsonSerializer _keepDefaultValuesSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
