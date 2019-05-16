using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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

        internal Session(int id)
        {
            Id = id;
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
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearAuthCache-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearAuthCache-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearAuthCache", Id, JObject.FromObject(options, _jsonSerializer), guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Clears the session’s HTTP authentication cache.
        /// </summary>
        /// <param name="options"></param>
        public Task ClearAuthCacheAsync(RemoveClientCertificate options)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearAuthCache-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearAuthCache-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearAuthCache", Id, JObject.FromObject(options, _jsonSerializer), guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Clears the session’s HTTP cache.
        /// </summary>
        /// <returns></returns>
        public Task ClearCacheAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearCache-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearCache-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearCache", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Clears the host resolver cache.
        /// </summary>
        /// <returns></returns>
        public Task ClearHostResolverCacheAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearHostResolverCache-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearHostResolverCache-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearHostResolverCache", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <returns></returns>
        public Task ClearStorageDataAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearStorageData-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearStorageData-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearStorageData", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task ClearStorageDataAsync(ClearStorageDataOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-clearStorageData-options-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-clearStorageData-options-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-clearStorageData-options", Id, JObject.FromObject(options, _jsonSerializer), guid);

            return taskCompletionSource.Task;
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
            BridgeConnector.Socket.Emit("webContents-session-createInterruptedDownload", Id, JObject.FromObject(options, _jsonSerializer));
        }

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
            BridgeConnector.Socket.Emit("webContents-session-enableNetworkEmulation", Id, JObject.FromObject(options, _jsonSerializer));
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
            var taskCompletionSource = new TaskCompletionSource<int[]>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-getBlobData-completed" + guid, (buffer) =>
            {
                var result = ((JArray)buffer).ToObject<int[]>();

                BridgeConnector.Socket.Off("webContents-session-getBlobData-completed" + guid);
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Socket.Emit("webContents-session-getBlobData", Id, identifier, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Get session's current cache size.
        /// </summary>
        /// <returns>Callback is invoked with the session's current cache size.</returns>
        public Task<int> GetCacheSizeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-getCacheSize-completed" + guid, (size) =>
            {
                BridgeConnector.Socket.Off("webContents-session-getCacheSize-completed" + guid);
                taskCompletionSource.SetResult((int)size);
            });

            BridgeConnector.Socket.Emit("webContents-session-getCacheSize", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string[]> GetPreloadsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string[]>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-getPreloads-completed" + guid, (preloads) =>
            {
                var result = ((JArray)preloads).ToObject<string[]>();
                BridgeConnector.Socket.Off("webContents-session-getPreloads-completed" + guid);
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Socket.Emit("webContents-session-getPreloads", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> GetUserAgent()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-getUserAgent-completed" + guid, (userAgent) =>
            {
                BridgeConnector.Socket.Off("webContents-session-getUserAgent-completed" + guid);
                taskCompletionSource.SetResult(userAgent.ToString());
            });

            BridgeConnector.Socket.Emit("webContents-session-getUserAgent", Id, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resolves the proxy information for url. The callback will be called with
        /// callback(proxy) when the request is performed.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> ResolveProxyAsync(string url)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-resolveProxy-completed" + guid, (proxy) =>
            {
                BridgeConnector.Socket.Off("webContents-session-resolveProxy-completed" + guid);
                taskCompletionSource.SetResult(proxy.ToString());
            });

            BridgeConnector.Socket.Emit("webContents-session-resolveProxy", Id, url, guid);

            return taskCompletionSource.Task;
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
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("webContents-session-setProxy-completed" + guid, () =>
            {
                BridgeConnector.Socket.Off("webContents-session-setProxy-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("webContents-session-setProxy", Id, JObject.FromObject(config, _jsonSerializer), guid);

            return taskCompletionSource.Task;
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

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
