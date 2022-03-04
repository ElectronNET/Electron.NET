using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
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

        /// <summary>
        /// Query and modify a session's cookies.
        /// </summary>
        public Cookies Cookies { get; }

        internal Session(int id)
        {
            Id = id;
            Cookies = new Cookies(id);
        }

        /// <summary>
        /// Dynamically sets whether to always send credentials for HTTP NTLM or Negotiate authentication.
        /// </summary>
        /// <param name="domains">A comma-separated list of servers for which integrated authentication is enabled.</param>
        public async Task AllowNTLMCredentialsForDomains(string domains)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-allowNTLMCredentialsForDomains", Id, domains);
        }

        /// <summary>
        /// Clears the session’s HTTP authentication cache.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> ClearAuthCacheAsync(RemovePassword options)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearAuthCache", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Clears the session’s HTTP authentication cache.
        /// </summary>
        public async Task<bool> ClearAuthCacheAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearAuthCache", Id);
        }

        /// <summary>
        /// Clears the session’s HTTP cache.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearCacheAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearCache", Id);
        }

        /// <summary>
        /// Clears the host resolver cache.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearHostResolverCacheAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearHostResolverCache", Id);
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearStorageDataAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearStorageData", Id);
        }

        /// <summary>
        /// Clears the data of web storages.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> ClearStorageDataAsync(ClearStorageDataOptions options)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-clearStorageData-options", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Allows resuming cancelled or interrupted downloads from previous Session. The
        /// API will generate a DownloadItem that can be accessed with the will-download
        /// event. The DownloadItem will not have any WebContents associated with it and the
        /// initial state will be interrupted. The download will start only when the resume
        /// API is called on the DownloadItem.
        /// </summary>
        /// <param name="options"></param>
        public async Task CreateInterruptedDownload(CreateInterruptedDownloadOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-createInterruptedDownload", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Disables any network emulation already active for the session. Resets to the
        /// original network configuration.
        /// </summary>
        public async Task DisableNetworkEmulation()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-disableNetworkEmulation", Id);
        }

        /// <summary>
        /// Emulates network with the given configuration for the session.
        /// </summary>
        /// <param name="options"></param>
        public async Task EnableNetworkEmulation(EnableNetworkEmulationOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-enableNetworkEmulation", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Writes any unwritten DOMStorage data to disk.
        /// </summary>
        public async Task FlushStorageData()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-flushStorageData", Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<int[]> GetBlobDataAsync(string identifier)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("webContents-session-getBlobData", Id, identifier);
            return signalrResult.ToObject<int[]>();
        }

        /// <summary>
        /// Get session's current cache size.
        /// </summary>
        /// <returns>Callback is invoked with the session's current cache size.</returns>
        public async Task<int> GetCacheSizeAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultInt("webContents-session-getCacheSize", Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetPreloadsAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("webContents-session-getPreloads", Id);
            return signalrResult.ToObject<string[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUserAgent()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("webContents-session-getUserAgent", Id);
        }

        /// <summary>
        /// Resolves the proxy information for url. The callback will be called with
        /// callback(proxy) when the request is performed.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> ResolveProxyAsync(string url)
        {
            return await SignalrSerializeHelper.GetSignalrResultString("webContents-session-resolveProxy", Id, url);
        }

        /// <summary>
        /// Sets download saving directory. By default, the download directory will be the
        /// Downloads under the respective app folder.
        /// </summary>
        /// <param name="path"></param>
        public async Task SetDownloadPath(string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-setDownloadPath", Id, path);
        }

        /// <summary>
        /// Adds scripts that will be executed on ALL web contents that are associated with
        /// this session just before normal preload scripts run.
        /// </summary>
        /// <param name="preloads"></param>
        public async Task SetPreloads(string[] preloads)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-setPreloads", Id, preloads);
        }

        /// <summary>
        /// Sets the proxy settings. When pacScript and proxyRules are provided together,
        /// the proxyRules option is ignored and pacScript configuration is applied.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<bool> SetProxyAsync(ProxyConfig config)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("webContents-session-setProxy", Id, JObject.FromObject(config, _jsonSerializer));
        }

        /// <summary>
        /// Overrides the userAgent for this session. This doesn't affect existing WebContents, and
        /// each WebContents can use webContents.setUserAgent to override the session-wide
        /// user agent.
        /// </summary>
        /// <param name="userAgent"></param>
        public async Task SetUserAgent(string userAgent)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-setUserAgent", Id, userAgent);
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
        public async Task SetUserAgent(string userAgent, string acceptLanguages)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-setUserAgent", Id, userAgent, acceptLanguages);
        }

        /// <summary>
        /// The keys are the extension names and each value is an object containing name and version properties.
        /// Note: This API cannot be called before the ready event of the app module is emitted.
        /// </summary>
        /// <returns></returns>
        public async Task<ChromeExtensionInfo[]> GetAllExtensionsAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("webContents-session-getAllExtensions", Id);
            return signalrResult.ToObject<ChromeExtensionInfo[]>();
        }

        /// <summary>
        /// Remove Chrome extension with the specified name.
        /// Note: This API cannot be called before the ready event of the app module is emitted.
        /// </summary>
        /// <param name="name">Name of the Chrome extension to remove</param>
        public async Task RemoveExtension(string name)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-removeExtension", Id, name);
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
        public async Task<Extension> LoadExtensionAsync(string path, bool allowFileAccess = false)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("webContents-session-loadExtension", Id, path, allowFileAccess);
            return signalrResult.ToObject<Extension>();
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
