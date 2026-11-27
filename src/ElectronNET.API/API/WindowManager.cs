using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
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
            EnsureBrowserWindowClosedSubscription();
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
        /// Gets the web contents views.
        /// </summary>
        /// <value>
        /// The web contents views.
        /// </value>
        public IReadOnlyCollection<WebContentsView> WebContentsViews
        {
            get
            {
                lock (_webContentsViews)
                {
                    return _webContentsViews.ToArray();
                }
            }
        }

        private readonly List<WebContentsView> _webContentsViews = new();

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
            var tcs = new TaskCompletionSource<BrowserWindow>();

            BridgeConnector.Socket.Once<int>("BrowserWindowCreated", (id) =>
            {
                var browserWindow = new BrowserWindow(id);
                _browserWindows.Add(browserWindow);

                tcs.SetResult(browserWindow);
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

            if (!options.X.HasValue && !options.Y.HasValue)
            {
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

            return await tcs.Task.ConfigureAwait(false);
        }

        private readonly object _browserWindowSubscriptionSync = new();
        private bool _browserWindowClosedSubscribed;

        private void EnsureBrowserWindowClosedSubscription()
        {
            if (_browserWindowClosedSubscribed)
            {
                return;
            }

            lock (_browserWindowSubscriptionSync)
            {
                if (_browserWindowClosedSubscribed)
                {
                    return;
                }

                BridgeConnector.Socket.On<int[]>("BrowserWindowClosed", HandleBrowserWindowClosed);
                BridgeConnector.Socket.On<int>("BrowserWindowRecreated", HandleBrowserWindowRecreated);
                _browserWindowClosedSubscribed = true;
            }
        }

        /// <summary>
        /// Emitted when Electron recreated the main window, e.g. when the app is activated again on macOS
        /// after all of its windows were closed. The startup callback passed to <c>UseElectron</c> is not
        /// run again in this case, so use this event to (re)wire anything that depends on the main window.
        /// </summary>
        public event Action<BrowserWindow> MainWindowRecreated;

        private void HandleBrowserWindowRecreated(int id)
        {
            var window = _browserWindows.FirstOrDefault(entry => entry.Id == id);

            if (window == null)
            {
                window = new BrowserWindow(id);
                _browserWindows.Add(window);
            }

            MainWindowRecreated?.Invoke(window);
        }

        private void HandleBrowserWindowClosed(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                _browserWindows.Clear();
                return;
            }

            var existingIds = ids.ToHashSet();
            _browserWindows.RemoveAll(window => !existingIds.Contains(window.Id));
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
            var tcs = new TaskCompletionSource<BrowserView>();

            BridgeConnector.Socket.Once<int>("BrowserViewCreated", (id) =>
            {
                BrowserView browserView = new(id);

                _browserViews.Add(browserView);

                tcs.SetResult(browserView);
            });

            await BridgeConnector.Socket.Emit("createBrowserView", options).ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// A WebContentsView can be used to embed additional web content into a BrowserWindow.
        /// It is the successor of BrowserView.
        /// </summary>
        /// <returns></returns>
        public Task<WebContentsView> CreateWebContentsViewAsync()
        {
            return CreateWebContentsViewAsync(new WebContentsViewConstructorOptions());
        }

        /// <summary>
        /// A WebContentsView can be used to embed additional web content into a BrowserWindow.
        /// It is the successor of BrowserView.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<WebContentsView> CreateWebContentsViewAsync(WebContentsViewConstructorOptions options)
        {
            await WebContentsViewCreationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var tcs = new TaskCompletionSource<WebContentsView>(TaskCreationOptions.RunContinuationsAsynchronously);

                BridgeConnector.Socket.Once<int>("WebContentsViewCreated", (id) =>
                {
                    WebContentsView view = new(id);

                    lock (_webContentsViews)
                    {
                        _webContentsViews.Add(view);
                    }

                    tcs.SetResult(view);
                });

                await BridgeConnector.Socket.Emit("createWebContentsView", options).ConfigureAwait(false);

                return await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                WebContentsViewCreationLock.Release();
            }
        }

        private static readonly System.Threading.SemaphoreSlim WebContentsViewCreationLock = new(1, 1);

        internal void RemoveWebContentsView(WebContentsView view)
        {
            lock (_webContentsViews)
            {
                _webContentsViews.Remove(view);
            }
        }
    }
}