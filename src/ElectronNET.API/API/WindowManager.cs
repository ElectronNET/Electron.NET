using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WindowManager
    {
        private static WindowManager _windowManager;
        private static readonly object SyncRoot = new();

        internal WindowManager()
        {
        }

        internal static WindowManager Instance
        {
            get
            {
                if (_windowManager == null)
                {
                    lock (SyncRoot)
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
            get => _isQuitOnWindowAllClosed;
            set
            {
                BridgeConnector.Socket.Emit("quit-app-window-all-closed", value);
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
        public IReadOnlyCollection<BrowserWindow> BrowserWindows => _browserWindows.AsReadOnly();

        private readonly List<BrowserWindow> _browserWindows = new();

        /// <summary>
        /// Gets the browser views.
        /// </summary>
        /// <value>
        /// The browser view.
        /// </value>
        public IReadOnlyCollection<BrowserView> BrowserViews => _browserViews.AsReadOnly();

        private readonly List<BrowserView> _browserViews = new();

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        public async Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost")
        {
            return await this.CreateWindowAsync(new BrowserWindowOptions(), loadUrl).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        public async Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost")
        {
            var taskCompletionSource = new TaskCompletionSource<BrowserWindow>();

            BridgeConnector.Socket.On<JsonElement>("BrowserWindowCreated", (id) =>
            {
                BridgeConnector.Socket.Off("BrowserWindowCreated");

                var browserWindowId = id.GetInt32();

                var browserWindow = new BrowserWindow(browserWindowId);
                _browserWindows.Add(browserWindow);

                taskCompletionSource.SetResult(browserWindow);
            });

            BridgeConnector.Socket.On<JsonElement>("BrowserWindowClosed", (ids) =>
            {
                BridgeConnector.Socket.Off("BrowserWindowClosed");

                var browserWindowIds = ids.Deserialize<int[]>(Serialization.ElectronJson.Options);

                for (int index = 0; index < _browserWindows.Count; index++)
                {
                    if (!browserWindowIds.Contains(_browserWindows[index].Id))
                    {
                        _browserWindows.RemoveAt(index);
                    }
                }
            });

            if (loadUrl.Equals("http://localhost", StringComparison.OrdinalIgnoreCase) && ElectronNetRuntime.AspNetWebPort.HasValue)
            {
                loadUrl = $"{loadUrl}:{ElectronNetRuntime.AspNetWebPort}";
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
                options.X = 0;
                options.Y = 0;

                await BridgeConnector.Socket.Emit("createBrowserWindow", options, loadUrl).ConfigureAwait(false);
            }
            else
            {
                // Workaround Windows 10 / Electron Bug
                // https://github.com/electron/electron/issues/4045
                if (IsWindows10())
                {
                    options.X -= 7;
                }

                await BridgeConnector.Socket.Emit("createBrowserWindow", options, loadUrl).ConfigureAwait(false);
            }

            return await taskCompletionSource.Task.ConfigureAwait(false);
        }

        private bool IsWindows10()
        {
            return RuntimeInformation.OSDescription.Contains("Windows 10");
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
            var taskCompletionSource = new TaskCompletionSource<BrowserView>();

            BridgeConnector.Socket.On<JsonElement>("BrowserViewCreated", (id) =>
            {
                BridgeConnector.Socket.Off("BrowserViewCreated");

                var browserViewId = id.GetInt32();
                BrowserView browserView = new(browserViewId);

                _browserViews.Add(browserView);

                taskCompletionSource.SetResult(browserView);
            });

            await BridgeConnector.Socket.Emit("createBrowserView", options).ConfigureAwait(false);

            return await taskCompletionSource.Task.ConfigureAwait(false);
        }

    }
}