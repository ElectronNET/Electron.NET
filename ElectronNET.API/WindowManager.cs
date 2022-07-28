using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WindowManager : IWindowManager
    {
        private static WindowManager _windowManager;
        private static readonly object _syncRoot = new();

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

        /// <summary>
        /// Get a browser window using the ID
        /// </summary>
        /// <param name="id">The id of the browser window</param>
        /// <param name="window">The window, if any</param>
        /// <returns>True if it found the window</returns>
        public bool TryGetBrowserWindows(int id, out BrowserWindow window) => _browserWindows.TryGetValue(id, out window);


        private readonly ConcurrentDictionary<int, BrowserWindow> _browserWindows = new ();

        /// <summary>
        /// Gets the browser views.
        /// </summary>
        /// <value>
        /// The browser view.
        /// </value>
        public IReadOnlyCollection<BrowserView> BrowserViews { get { return _browserViews.Values.ToList().AsReadOnly(); } }
        private readonly ConcurrentDictionary<int, BrowserView> _browserViews = new ();

        /// <summary>
        /// Get a browser view using the ID
        /// </summary>
        /// <param name="id">The id of the browser view</param>
        /// <param name="view">The view, if any</param>
        /// <returns>True if it found the view</returns>
        public bool TryGetBrowserViews(int id, out BrowserView view) => _browserViews.TryGetValue(id, out view);

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        public async Task<BrowserWindow> CreateWindowAsync(string loadUrl = "/")
        {
            return await CreateWindowAsync(new BrowserWindowOptions(), loadUrl);
        }

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        public async Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost")
        {
            BootstrapUpdateOpenIDsEvent();

            var taskCompletionSource = new TaskCompletionSource<BrowserWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Once<int>("BrowserWindowCreated" + guid, (id) =>
            {
                var browserWindow = new BrowserWindow(id);

                _browserWindows[id] = browserWindow;

                taskCompletionSource.SetResult(browserWindow);
            });

            if (string.Equals(loadUrl, "HTTP://LOCALHOST", StringComparison.InvariantCultureIgnoreCase))
            {
                loadUrl = $"{loadUrl}:{BridgeSettings.WebPort}";
            }

            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (IsWindows10())
            {
                options.Width += 14;
                options.Height += 7;
            }

            if (options.X == -1 && options.Y == -1)
            {
                options.X = 0; //This is manually removed by the browserWindows.js code before creating the window
                options.Y = 0;

                BridgeConnector.Emit("createBrowserWindow", guid, JObject.FromObject(options, _keepDefaultValuesSerializer), loadUrl);
            }
            else
            {
                // Workaround Windows 10 / Electron Bug
                // https://github.com/electron/electron/issues/4045
                if (IsWindows10())
                {
                    options.X -= 7;
                }

                BridgeConnector.Emit("createBrowserWindow", guid, JObject.FromObject(options, _keepDefaultValuesSerializer), loadUrl);
            }

            return await taskCompletionSource.Task;
        }


        private bool _hasClosedEvent = false;
        private readonly object _hasClosedEventLock  = new();
        private void BootstrapUpdateOpenIDsEvent()
        {
            if (!_hasClosedEvent)
            {
                lock(_hasClosedEventLock)
                {
                    if(!_hasClosedEvent)
                    {
                        BridgeConnector.On<int[]>("BrowserWindowUpdateOpenIDs", (browserWindowIdsStillOpen) =>
                        {
                            if (browserWindowIdsStillOpen.Any())
                            {
                                foreach (var id in _browserWindows.Keys.ToArray())
                                {
                                    if (!browserWindowIdsStillOpen.Contains(id)) _browserWindows.TryRemove(id, out _);
                                }
                            }
                            else
                            {
                                _browserWindows.Clear();
                            }
                        });
                        _hasClosedEvent = true;
                    }
                }    
            }
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
        public async Task<BrowserView> CreateBrowserViewAsync(BrowserViewConstructorOptions options)
        {
            var guid = Guid.NewGuid().ToString();

            var taskCompletionSource = new TaskCompletionSource<BrowserView>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.Once<int>("BrowserViewCreated" + guid, (id) =>
            {
                var browserView = new BrowserView(id);
                _browserViews[id] = browserView;
                taskCompletionSource.SetResult(browserView);
            });

            BridgeConnector.Emit("createBrowserView", guid, JObject.FromObject(options, _keepDefaultValuesSerializer));

            return await taskCompletionSource.Task;
        }


        /// <summary>
        /// Destroy all windows.
        /// </summary>
        /// <returns>Number of windows destroyed</returns>
        public async Task<int> DestroyAllWindows()
        {
            var destroyed = await BridgeConnector.OnResult<int>("browserWindowDestroyAll", "browserWindowDestroyAll-completed");
            _browserViews.Clear();
            _browserWindows.Clear();
            return destroyed;
        }

        private static readonly JsonSerializer _keepDefaultValuesSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
