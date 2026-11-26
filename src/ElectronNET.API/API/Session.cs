using ElectronNET.API.Entities;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Manage browser sessions, cookies, cache, proxy settings, etc.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Query and modify a session's cookies.
        /// </summary>
        public Cookies Cookies { get; }

        public WebRequest WebRequest { get; private set; }

        internal Session(int id)
        {
            Id = id;
            Cookies = new Cookies(id);
            WebRequest = new WebRequest(id);
        }

        /// <summary>
        /// Dynamically sets whether to always send credentials for HTTP NTLM or Negotiate authentication.
        /// </summary>
        /// <param name="domains">A comma-separated list of servers for which integrated authentication is enabled.</param>
        public void AllowNTLMCredentialsForDomains(string domains)
        {
            BridgeConnector.Socket.Emit("webContents-session-allowNTLMCredentialsForDomains", Id, domains);
        }

        /// <summary>
        /// Clears the session’s HTTP authentication cache.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task ClearAuthCacheAsync(RemovePassword options)
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearAuthCache-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearAuthCache", Id, options, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Clears the session’s HTTP authentication cache.
        /// </summary>
        public Task ClearAuthCacheAsync()
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearAuthCache-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearAuthCache", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Clears the session’s HTTP cache.
        /// </summary>
        /// <returns></returns>
        public Task ClearCacheAsync()
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearCache-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearCache", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Clears the host resolver cache.
        /// </summary>
        /// <returns></returns>
        public Task ClearHostResolverCacheAsync()
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearHostResolverCache-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearHostResolverCache", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <returns></returns>
        public Task ClearStorageDataAsync()
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearStorageData-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearStorageData", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task ClearStorageDataAsync(ClearStorageDataOptions options)
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-clearStorageData-options-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-clearStorageData-options", Id, options, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Allows resuming cancelled or interrupted downloads from previous Session. The
        /// API will generate a DownloadItem that can be accessed with the will-download
        /// event. The DownloadItem will not have any WebContents associated with it and the
        /// initial state will be interrupted. The download will start only when the resume
        /// API is called on the DownloadItem.
        /// </summary>
        /// <param name="options"></param>
        public void CreateInterruptedDownload(CreateInterruptedDownloadOptions options)
        {
            BridgeConnector.Socket.Emit("webContents-session-createInterruptedDownload", Id, options);
        }

        /// <summary>
        /// Sets the rules that decide how this session handles downloads, e.g. to block
        /// certain file extensions or to store downloads in a directory without showing
        /// a save dialog. The rules are evaluated synchronously in the Electron host, so
        /// they take effect for every download. Pass null to restore Electron's default
        /// behavior.
        /// <para>
        /// The policy belongs to the underlying Electron session, which by default is shared
        /// by all windows.
        /// </para>
        /// </summary>
        /// <param name="policy">The download rules, or null to remove them.</param>
        public void SetDownloadPolicy(DownloadPolicy policy)
        {
            BridgeConnector.Socket.Emit("webContents-session-setDownloadPolicy", Id, policy);
        }

        /// <summary>
        /// Emitted when a download that is allowed by the <see cref="DownloadPolicy"/>
        /// (or any download, if no policy is set) starts.
        /// </summary>
        public event Action<DownloadInfo> OnDownloadStarted
        {
            add
            {
                if (_downloadStarted == null)
                {
                    BridgeConnector.Socket.On<DownloadInfo>("webContents-session-download-started" + Id, info => _downloadStarted?.Invoke(info));
                    BridgeConnector.Socket.Emit("register-webContents-session-download", Id, "started");
                }

                _downloadStarted += value;
            }
            remove
            {
                _downloadStarted -= value;

                if (_downloadStarted == null)
                {
                    BridgeConnector.Socket.Off("webContents-session-download-started" + Id);
                    BridgeConnector.Socket.Emit("unregister-webContents-session-download", Id, "started");
                }
            }
        }

        private event Action<DownloadInfo> _downloadStarted;

        /// <summary>
        /// Emitted when a download was blocked by the <see cref="DownloadPolicy"/>.
        /// </summary>
        public event Action<DownloadInfo> OnDownloadBlocked
        {
            add
            {
                if (_downloadBlocked == null)
                {
                    BridgeConnector.Socket.On<DownloadInfo>("webContents-session-download-blocked" + Id, info => _downloadBlocked?.Invoke(info));
                    BridgeConnector.Socket.Emit("register-webContents-session-download", Id, "blocked");
                }

                _downloadBlocked += value;
            }
            remove
            {
                _downloadBlocked -= value;

                if (_downloadBlocked == null)
                {
                    BridgeConnector.Socket.Off("webContents-session-download-blocked" + Id);
                    BridgeConnector.Socket.Emit("unregister-webContents-session-download", Id, "blocked");
                }
            }
        }

        private event Action<DownloadInfo> _downloadBlocked;

        /// <summary>
        /// Emitted when a download has completed, was cancelled or was interrupted.
        /// </summary>
        public event Action<DownloadDoneInfo> OnDownloadDone
        {
            add
            {
                if (_downloadDone == null)
                {
                    BridgeConnector.Socket.On<DownloadDoneInfo>("webContents-session-download-done" + Id, info => _downloadDone?.Invoke(info));
                    BridgeConnector.Socket.Emit("register-webContents-session-download", Id, "done");
                }

                _downloadDone += value;
            }
            remove
            {
                _downloadDone -= value;

                if (_downloadDone == null)
                {
                    BridgeConnector.Socket.Off("webContents-session-download-done" + Id);
                    BridgeConnector.Socket.Emit("unregister-webContents-session-download", Id, "done");
                }
            }
        }

        private event Action<DownloadDoneInfo> _downloadDone;

        /// <summary>
        /// Disables any network emulation already active for the session. Resets to the
        /// original network configuration.
        /// </summary>
        public void DisableNetworkEmulation()
        {
            BridgeConnector.Socket.Emit("webContents-session-disableNetworkEmulation", Id);
        }

        /// <summary>
        /// Emulates network with the given configuration for the session.
        /// </summary>
        /// <param name="options"></param>
        public void EnableNetworkEmulation(EnableNetworkEmulationOptions options)
        {
            BridgeConnector.Socket.Emit("webContents-session-enableNetworkEmulation", Id, options);
        }

        /// <summary>
        /// Writes any unwritten DOMStorage data to disk.
        /// </summary>
        public void FlushStorageData()
        {
            BridgeConnector.Socket.Emit("webContents-session-flushStorageData", Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public Task<int[]> GetBlobDataAsync(string identifier)
        {
            var tcs = new TaskCompletionSource<int[]>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<int[]>("webContents-session-getBlobData-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-getBlobData", Id, identifier, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Get session's current cache size.
        /// </summary>
        /// <returns>Callback is invoked with the session's current cache size.</returns>
        public Task<int> GetCacheSizeAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<int>("webContents-session-getCacheSize-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-getCacheSize", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string[]> GetPreloadsAsync()
        {
            var tcs = new TaskCompletionSource<string[]>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string[]>("webContents-session-getPreloads-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-getPreloads", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> GetUserAgent()
        {
            var tcs = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string>("webContents-session-getUserAgent-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-getUserAgent", Id, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Resolves the proxy information for url. The callback will be called with
        /// callback(proxy) when the request is performed.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> ResolveProxyAsync(string url)
        {
            var tcs = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string>("webContents-session-resolveProxy-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-resolveProxy", Id, url, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Sets download saving directory. By default, the download directory will be the
        /// Downloads under the respective app folder.
        /// </summary>
        /// <param name="path"></param>
        public void SetDownloadPath(string path)
        {
            BridgeConnector.Socket.Emit("webContents-session-setDownloadPath", Id, path);
        }

        /// <summary>
        /// Adds scripts that will be executed on ALL web contents that are associated with
        /// this session just before normal preload scripts run.
        /// </summary>
        /// <param name="preloads"></param>
        public void SetPreloads(string[] preloads)
        {
            BridgeConnector.Socket.Emit("webContents-session-setPreloads", Id, preloads);
        }

        /// <summary>
        /// Sets the proxy settings. When pacScript and proxyRules are provided together,
        /// the proxyRules option is ignored and pacScript configuration is applied.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Task SetProxyAsync(ProxyConfig config)
        {
            var tcs = new TaskCompletionSource();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once("webContents-session-setProxy-completed" + guid, () => tcs.SetResult());
            BridgeConnector.Socket.Emit("webContents-session-setProxy", Id, config, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Overrides the userAgent for this session. This doesn't affect existing WebContents, and
        /// each WebContents can use webContents.setUserAgent to override the session-wide
        /// user agent.
        /// </summary>
        /// <param name="userAgent"></param>
        public void SetUserAgent(string userAgent)
        {
            BridgeConnector.Socket.Emit("webContents-session-setUserAgent", Id, userAgent);
        }

        /// <summary>
        /// Overrides the userAgent and acceptLanguages for this session. The
        /// acceptLanguages must a comma separated ordered list of language codes, for
        /// example "en-US,fr,de,ko,zh-CN,ja". This doesn't affect existing WebContents, and
        /// each WebContents can use webContents.setUserAgent to override the session-wide
        /// user agent.
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="acceptLanguages">The
        /// acceptLanguages must a comma separated ordered list of language codes, for
        /// example "en-US,fr,de,ko,zh-CN,ja".</param>
        public void SetUserAgent(string userAgent, string acceptLanguages)
        {
            BridgeConnector.Socket.Emit("webContents-session-setUserAgent", Id, userAgent, acceptLanguages);
        }

        /// <summary>
        /// The keys are the extension names and each value is an object containing name and version properties.
        /// Note: This API cannot be called before the ready event of the app module is emitted.
        /// </summary>
        /// <returns></returns>
        public Task<ChromeExtensionInfo[]> GetAllExtensionsAsync()
        {
            var tcs = new TaskCompletionSource<ChromeExtensionInfo[]>();

            BridgeConnector.Socket.Once<ChromeExtensionInfo[]>("webContents-session-getAllExtensions-completed", tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-getAllExtensions", Id);

            return tcs.Task;
        }

        /// <summary>
        /// Remove Chrome extension with the specified name.
        /// Note: This API cannot be called before the ready event of the app module is emitted.
        /// </summary>
        /// <param name="name">Name of the Chrome extension to remove</param>
        public void RemoveExtension(string name)
        {
            BridgeConnector.Socket.Emit("webContents-session-removeExtension", Id, name);
        }

        /// <summary>
        /// resolves when the extension is loaded.
        ///
        /// This method will raise an exception if the extension could not be loaded.If
        /// there are warnings when installing the extension (e.g. if the extension requests
        /// an API that Electron does not support) then they will be logged to the console.
        ///
        /// Note that Electron does not support the full range of Chrome extensions APIs.
        /// See Supported Extensions APIs for more details on what is supported.
        ///
        /// Note that in previous versions of Electron, extensions that were loaded would be
        /// remembered for future runs of the application.This is no longer the case:
        /// `loadExtension` must be called on every boot of your app if you want the
        /// extension to be loaded.
        ///
        /// This API does not support loading packed (.crx) extensions.
        ///
        ///** Note:** This API cannot be called before the `ready` event of the `app` module
        /// is emitted.
        ///
        ///** Note:** Loading extensions into in-memory(non-persistent) sessions is not supported and will throw an error.
        /// </summary>
        /// <param name="path">Path to the Chrome extension</param>
        /// <param name="allowFileAccess">Whether to allow the extension to read local files over `file://` protocol and
        /// inject content scripts into `file://` pages. This is required e.g. for loading
        /// devtools extensions on `file://` URLs. Defaults to false.</param>
        /// <returns></returns>
        public Task<Extension> LoadExtensionAsync(string path, bool allowFileAccess = false)
        {
            var tcs = new TaskCompletionSource<Extension>();

            BridgeConnector.Socket.Once<Extension>("webContents-session-loadExtension-completed", tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-loadExtension", Id, path, allowFileAccess);

            return tcs.Task;
        }
    }
}