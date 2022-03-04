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
    /// Render and control web pages.
    /// </summary>
    public class WebContents
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Manage browser sessions, cookies, cache, proxy settings, etc.
        /// </summary>
        public Session Session { get; internal set; }

        /// <summary>
        /// Emitted when the renderer process crashes or is killed.
        /// </summary>
        public event Action<bool> OnCrashed
        {
            add
            {
                if (_crashed == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-webContents-crashed", Id);
                }
                _crashed += value;
            }
            remove
            {
                _crashed -= value;
            }
        }

        public void TriggerOnCrashed(bool crashed)
        {
            _crashed(crashed);
        }

        private event Action<bool> _crashed;

        /// <summary>
        /// Emitted when the navigation is done, i.e. the spinner of the tab has
        /// stopped spinning, and the onload event was dispatched.
        /// </summary>
        public event Action OnDidFinishLoad
        {
            add
            {
                if (_didFinishLoad == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-webContents-didFinishLoad", Id);
                }
                _didFinishLoad += value;
            }
            remove
            {
                _didFinishLoad -= value;
            }
        }

        public void TriggerOnDidFinishLoad()
        {
            _didFinishLoad();
        }

        private event Action _didFinishLoad;

        internal WebContents(int id)
        {
            Id = id;
            Session = new Session(id);
        }

        /// <summary>
        /// Opens the devtools.
        /// </summary>
        public async void OpenDevTools()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContentsOpenDevTools", Id);
        }

        /// <summary>
        /// Opens the devtools.
        /// </summary>
        /// <param name="openDevToolsOptions"></param>
        public async void OpenDevTools(OpenDevToolsOptions openDevToolsOptions)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContentsOpenDevTools", Id, JObject.FromObject(openDevToolsOptions, _jsonSerializer));
        }

        /// <summary>
        /// Get system printers.
        /// </summary>
        /// <returns>printers</returns>
        public async Task<PrinterInfo[]> GetPrintersAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("webContents-getPrinters", Id);
            return signalrResult.ToObject<PrinterInfo[]>();
        }

        /// <summary>
        /// Prints window's web page.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>success</returns>
        public async Task<bool> PrintAsync(PrintOptions options = null)
        {
            if (options == null)
            {
                return await SignalrSerializeHelper.GetSignalrResultBool("webContents-print", Id, new JObject());
            }
            else
            {
                return await SignalrSerializeHelper.GetSignalrResultBool("webContents-print", Id, JObject.FromObject(options, _jsonSerializer));
            }
        }

        /// <summary>
        /// Prints window's web page as PDF with Chromium's preview printing custom
        /// settings.The landscape will be ignored if @page CSS at-rule is used in the web page. 
        /// By default, an empty options will be regarded as: Use page-break-before: always; 
        /// CSS style to force to print to a new page.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <returns>success</returns>
        public async Task<bool> PrintToPDFAsync(string path, PrintToPDFOptions options = null)
        {
            if (options == null)
            {
                return await SignalrSerializeHelper.GetSignalrResultBool("webContents-printToPDF", Id, new JObject(), path);
            }
            else
            {
                return await SignalrSerializeHelper.GetSignalrResultBool("webContents-printToPDF", Id, JObject.FromObject(options, _jsonSerializer), path);
            }
        }

        /// <summary>
        /// Is used to get the Url of the loaded page.
        /// It's usefull if a web-server redirects you and you need to know where it redirects. For instance, It's useful in case of Implicit Authorization.
        /// </summary>
        /// <returns>URL of the loaded page</returns>
        public async Task<string> GetUrl()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("webContents-getUrl", Id);
        }

        /// <summary>
        /// The async method will resolve when the page has finished loading, 
        /// and rejects if the page fails to load.
        /// 
        /// A noop rejection handler is already attached, which avoids unhandled rejection
        /// errors.
        ///
        /// Loads the `url` in the window. The `url` must contain the protocol prefix, e.g.
        /// the `http://` or `file://`. If the load should bypass http cache then use the
        /// `pragma` header to achieve it.
        /// </summary>
        /// <param name="url"></param>
        public Task LoadURLAsync(string url)
        {
            return LoadURLAsync(url, new LoadURLOptions());
        }

        /// <summary>
        /// The async method will resolve when the page has finished loading, 
        /// and rejects if the page fails to load.
        /// 
        /// A noop rejection handler is already attached, which avoids unhandled rejection
        /// errors.
        ///
        /// Loads the `url` in the window. The `url` must contain the protocol prefix, e.g.
        /// the `http://` or `file://`. If the load should bypass http cache then use the
        /// `pragma` header to achieve it.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public async Task LoadURLAsync(string url, LoadURLOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            var signalrResult = await SignalrSerializeHelper.GetSignalrResultString("webContents-loadURL", Id, url, JObject.FromObject(options, _jsonSerializer));

            if (String.IsNullOrEmpty(signalrResult))
            {
                taskCompletionSource.SetResult(null);
            } else
            {
                taskCompletionSource.SetException(new InvalidOperationException(signalrResult.ToString()));
            }

            return;
        }

        /// <summary>
        /// Inserts CSS into the web page.
        /// See: https://www.electronjs.org/docs/api/web-contents#contentsinsertcsscss-options
        /// Works for both BrowserWindows and BrowserViews.
        /// </summary>
        /// <param name="isBrowserWindow">Whether the webContents belong to a BrowserWindow or not (the other option is a BrowserView)</param>
        /// <param name="path">Absolute path to the CSS file location</param>
        public async Task InsertCSS(bool isBrowserWindow, string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-insertCSS", Id, isBrowserWindow, path);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}