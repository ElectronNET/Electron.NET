using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API;

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
                BridgeConnector.Socket.On("webContents-crashed" + Id, (killed) =>
                {
                    _crashed((bool)killed);
                });

                BridgeConnector.Socket.Emit("register-webContents-crashed", Id);
            }
            _crashed += value;
        }
        remove
        {
            _crashed -= value;

            if (_crashed == null)
                BridgeConnector.Socket.Off("webContents-crashed" + Id);
        }
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
                BridgeConnector.Socket.On("webContents-didFinishLoad" + Id, () =>
                {
                    _didFinishLoad();
                });

                BridgeConnector.Socket.Emit("register-webContents-didFinishLoad", Id);
            }
            _didFinishLoad += value;
        }
        remove
        {
            _didFinishLoad -= value;

            if (_didFinishLoad == null)
                BridgeConnector.Socket.Off("webContents-didFinishLoad" + Id);
        }
    }

    private event Action _didFinishLoad;

    /// <summary>
    /// Emitted when any frame (including main) starts navigating.
    /// </summary>
    public event Action<string> OnDidStartNavigation
    {
        add
        {
            if (_didStartNavigation == null)
            {
                BridgeConnector.Socket.On<string>("webContents-didStartNavigation" + Id, (url) =>
                {
                    _didStartNavigation(url);
                });

                BridgeConnector.Socket.Emit("register-webContents-didStartNavigation", Id);
            }
            _didStartNavigation += value;
        }
        remove
        {
            _didStartNavigation -= value;

            if (_didStartNavigation == null)
                BridgeConnector.Socket.Off("webContents-didStartNavigation" + Id);
        }
    }

    private event Action<string> _didStartNavigation;

    /// <summary>
    /// Emitted when a main frame navigation is done.
    /// This event is not emitted for in-page navigations, such as clicking anchor links or updating the window.location.hash. Use did-navigate-in-page event for this purpose.
    /// </summary>
    public event Action<OnDidNavigateInfo> OnDidNavigate
    {
        add
        {
            if (_didNavigate == null)
            {
                BridgeConnector.Socket.On<OnDidNavigateInfo>("webContents-didNavigate" + Id, (data) =>
                {
                    _didNavigate(data);
                });

                BridgeConnector.Socket.Emit("register-webContents-didNavigate", Id);
            }
            _didNavigate += value;
        }
        remove
        {
            _didNavigate -= value;

            if (_didNavigate == null)
                BridgeConnector.Socket.Off("webContents-didNavigate" + Id);
        }
    }

    private event Action<OnDidNavigateInfo> _didNavigate;

    /// <summary>
    /// Emitted when a server side redirect occurs during navigation. For example a 302 redirect.
    /// This event will be emitted after OnDidStartNavigation and always before the OnDidRedirectNavigation event for the same navigation.
    /// </summary>
    public event Action<string> OnWillRedirect
    {
        add
        {
            if (_willRedirect == null)
            {
                BridgeConnector.Socket.On<string>("webContents-willRedirect" + Id, (url) =>
                {
                    _willRedirect(url);
                });

                BridgeConnector.Socket.Emit("register-webContents-willRedirect", Id);
            }
            _willRedirect += value;
        }
        remove
        {
            _willRedirect -= value;

            if (_willRedirect == null)
                BridgeConnector.Socket.Off("webContents-willRedirect" + Id);
        }
    }

    private event Action<string> _willRedirect;

    /// <summary>
    /// Emitted after a server side redirect occurs during navigation. For example a 302 redirect.
    /// </summary>
    public event Action<string> OnDidRedirectNavigation
    {
        add
        {
            if (_didRedirectNavigation == null)
            {
                BridgeConnector.Socket.On("webContents-didRedirectNavigation" + Id, (url) =>
                {
                    _didRedirectNavigation(url?.ToString());
                });

                BridgeConnector.Socket.Emit("register-webContents-didRedirectNavigation", Id);
            }
            _didRedirectNavigation += value;
        }
        remove
        {
            _didRedirectNavigation -= value;

            if (_didRedirectNavigation == null)
                BridgeConnector.Socket.Off("webContents-didRedirectNavigation" + Id);
        }
    }

    private event Action<string> _didRedirectNavigation;


    /// <summary>
    /// This event is like OnDidFinishLoad but emitted when the load failed.
    /// </summary>
    public event Action<OnDidFailLoadInfo> OnDidFailLoad
    {
        add
        {
            if (_didFailLoad == null)
            {
                BridgeConnector.Socket.On("webContents-willRedirect" + Id, (data) =>
                {
                    _didFailLoad(((JObject) data).ToObject<OnDidFailLoadInfo>());
                });

                BridgeConnector.Socket.Emit("register-webContents-willRedirect", Id);
            }
            _didFailLoad += value;
        }
        remove
        {
            _didFailLoad -= value;

            if (_didFailLoad == null)
                BridgeConnector.Socket.Off("webContents-willRedirect" + Id);
        }
    }

    private event Action<OnDidFailLoadInfo> _didFailLoad;

    /// <summary>
    /// Emitted when an input event is sent to the WebContents.
    /// </summary>
    public event Action<InputEvent> InputEvent
    {
        add
        {
            if (_inputEvent == null)
            {
                BridgeConnector.Socket.On("webContents-input-event" + Id, (eventArgs) =>
                {
                    var inputEvent = ((JObject)eventArgs).ToObject<InputEvent>();
                    _inputEvent(inputEvent);
                });

                BridgeConnector.Socket.Emit("register-webContents-input-event", Id);
            }
            _inputEvent += value;
        }
        remove
        {
            _inputEvent -= value;

            if (_inputEvent == null)
                BridgeConnector.Socket.Off("webContents-input-event" + Id);
        }
    }

    private event Action<InputEvent> _inputEvent;

    /// <summary>
    /// Emitted when the document in the top-level frame is loaded.
    /// </summary>
    public event Action OnDomReady
    {
        add
        {
            if (_domReady == null)
            {
                BridgeConnector.Socket.On("webContents-domReady" + Id, () =>
                    {
                        _domReady();
                    });

                BridgeConnector.Socket.Emit("register-webContents-domReady", Id);
            }
            _domReady += value;
        }
        remove
        {
            _domReady -= value;

            if (_domReady == null)
                BridgeConnector.Socket.Off("webContents-domReady" + Id);
        }
    }

    private event Action _domReady;

    internal WebContents(int id)
    {
        Id = id;
        Session = new Session(id);
    }

    /// <summary>
    /// Opens the devtools.
    /// </summary>
    public void OpenDevTools()
    {
        BridgeConnector.Socket.Emit("webContentsOpenDevTools", Id);
    }

    /// <summary>
    /// Opens the devtools.
    /// </summary>
    /// <param name="openDevToolsOptions"></param>
    public void OpenDevTools(OpenDevToolsOptions openDevToolsOptions)
    {
        BridgeConnector.Socket.Emit("webContentsOpenDevTools", Id, JObject.FromObject(openDevToolsOptions, _jsonSerializer));
    }

    /// <summary>
    /// Get system printers.
    /// </summary>
    /// <returns>printers</returns>
    public Task<PrinterInfo[]> GetPrintersAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<PrinterInfo[]>();

        BridgeConnector.Socket.On("webContents-getPrinters-completed", (printers) =>
        {
            BridgeConnector.Socket.Off("webContents-getPrinters-completed");

            taskCompletionSource.SetResult(((Newtonsoft.Json.Linq.JArray)printers).ToObject<PrinterInfo[]>());
        });

        BridgeConnector.Socket.Emit("webContents-getPrinters", Id);

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Prints window's web page.
    /// </summary>
    /// <param name="options"></param>
    /// <returns>success</returns>
    public Task<bool> PrintAsync(PrintOptions options = null)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        BridgeConnector.Socket.On("webContents-print-completed", (success) =>
        {
            BridgeConnector.Socket.Off("webContents-print-completed");
            taskCompletionSource.SetResult((bool)success);
        });

        if(options == null)
        {
            BridgeConnector.Socket.Emit("webContents-print", Id, "");
        }
        else
        {
            BridgeConnector.Socket.Emit("webContents-print", Id, JObject.FromObject(options, _jsonSerializer));
        }

        return taskCompletionSource.Task;
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
    public Task<bool> PrintToPDFAsync(string path, PrintToPDFOptions options = null)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        BridgeConnector.Socket.On("webContents-printToPDF-completed", (success) =>
        {
            BridgeConnector.Socket.Off("webContents-printToPDF-completed");
            taskCompletionSource.SetResult((bool)success);
        });

        if(options == null)
        {
            BridgeConnector.Socket.Emit("webContents-printToPDF", Id, "", path);
        }
        else
        {
            BridgeConnector.Socket.Emit("webContents-printToPDF", Id, JObject.FromObject(options, _jsonSerializer), path);
        }

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Evaluates script code in page.
    /// </summary>
    /// <param name="code">The code to execute.</param>
    /// <param name="userGesture">if set to <c>true</c> simulate a user gesture.</param>
    /// <returns>The result of the executed code.</returns>
    /// <remarks>
    /// <para>
    /// In the browser window some HTML APIs like `requestFullScreen` can only be
    /// invoked by a gesture from the user. Setting `userGesture` to `true` will remove
    /// this limitation.
    /// </para>
    /// <para>
    /// Code execution will be suspended until web page stop loading.
    /// </para>
    /// </remarks>
    public Task<object> ExecuteJavaScriptAsync(string code, bool userGesture = false)
    {
        var taskCompletionSource = new TaskCompletionSource<object>();

        BridgeConnector.Socket.On("webContents-executeJavaScript-completed", (result) =>
        {
            BridgeConnector.Socket.Off("webContents-executeJavaScript-completed");
            taskCompletionSource.SetResult(result);
        });

        BridgeConnector.Socket.Emit("webContents-executeJavaScript", Id, code, userGesture);

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Is used to get the Url of the loaded page.
    /// It's usefull if a web-server redirects you and you need to know where it redirects. For instance, It's useful in case of Implicit Authorization.
    /// </summary>
    /// <returns>URL of the loaded page</returns>
    public Task<string> GetUrl()
    {
        var taskCompletionSource = new TaskCompletionSource<string>();

        var eventString = "webContents-getUrl" + Id;
        BridgeConnector.Socket.On(eventString, (url) =>
        {
            BridgeConnector.Socket.Off(eventString);
            taskCompletionSource.SetResult((string)url);
        });

        BridgeConnector.Socket.Emit("webContents-getUrl", Id);

        return taskCompletionSource.Task;
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
    public Task LoadURLAsync(string url, LoadURLOptions options)
    {
        var taskCompletionSource = new TaskCompletionSource<object>();

        BridgeConnector.Socket.On("webContents-loadURL-complete" + Id, () =>
        {
            BridgeConnector.Socket.Off("webContents-loadURL-complete" + Id);
            BridgeConnector.Socket.Off("webContents-loadURL-error" + Id);
            taskCompletionSource.SetResult(null);
        });

        BridgeConnector.Socket.On("webContents-loadURL-error" + Id, (error) =>
        {
            BridgeConnector.Socket.Off("webContents-loadURL-error" + Id);
            taskCompletionSource.SetException(new InvalidOperationException(error.ToString()));
        });

        BridgeConnector.Socket.Emit("webContents-loadURL", Id, url, JObject.FromObject(options, _jsonSerializer));

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Inserts CSS into the web page.
    /// See: https://www.electronjs.org/docs/api/web-contents#contentsinsertcsscss-options
    /// Works for both BrowserWindows and BrowserViews.
    /// </summary>
    /// <param name="isBrowserWindow">Whether the webContents belong to a BrowserWindow or not (the other option is a BrowserView)</param>
    /// <param name="path">Absolute path to the CSS file location</param>
    public void InsertCSS(bool isBrowserWindow, string path)
    {
        BridgeConnector.Socket.Emit("webContents-insertCSS", Id, isBrowserWindow, path);
    }

    private readonly JsonSerializer _jsonSerializer = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };
}