using ElectronNET.API.Entities;
using System;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API;

/// <summary>
/// Render and control web pages.
/// </summary>
public class WebContents : ApiBase
{
    protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
    protected override SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.DashesLowerFirst;
    protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.CamelCase;

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public override int Id { get; protected set; }

    /// <summary>
    /// Manage browser sessions, cookies, cache, proxy settings, etc.
    /// </summary>
    public Session Session { get; internal set; }

    /// <summary>
    /// Emitted when the renderer process crashes or is killed.
    /// </summary>
    public event Action<bool> OnCrashed
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when the navigation is done, i.e. the spinner of the tab has
    /// stopped spinning, and the onload event was dispatched.
    /// </summary>
    public event Action OnDidFinishLoad
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when any frame (including main) starts navigating.
    /// </summary>
    public event Action<string> OnDidStartNavigation
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when a main frame navigation is done.
    /// This event is not emitted for in-page navigations, such as clicking anchor links or updating the window.location.hash. Use did-navigate-in-page event for this purpose.
    /// </summary>
    public event Action<OnDidNavigateInfo> OnDidNavigate
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when a server side redirect occurs during navigation. For example a 302 redirect.
    /// This event will be emitted after OnDidStartNavigation and always before the OnDidRedirectNavigation event for the same navigation.
    /// </summary>
    public event Action<string> OnWillRedirect
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted after a server side redirect occurs during navigation. For example a 302 redirect.
    /// </summary>
    public event Action<string> OnDidRedirectNavigation
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// This event is like OnDidFinishLoad but emitted when the load failed.
    /// </summary>
    public event Action<OnDidFailLoadInfo> OnDidFailLoad
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when an input event is sent to the WebContents.
    /// </summary>
    public event Action<InputEvent> InputEvent
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

    /// <summary>
    /// Emitted when the document in the top-level frame is loaded.
    /// </summary>
    public event Action OnDomReady
    {
        add => AddEvent(value, Id);
        remove => RemoveEvent(value, Id);
    }

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
        BridgeConnector.Socket.Emit("webContentsOpenDevTools", Id, openDevToolsOptions);
    }

    /// <summary>
    /// Get system printers.
    /// </summary>
    /// <returns>printers</returns>
    public Task<PrinterInfo[]> GetPrintersAsync() => this.InvokeAsyncWithTimeout<PrinterInfo[]>(5_000);

    /// <summary>
    /// Prints window's web page.
    /// </summary>
    /// <param name="options"></param>
    /// <returns>success</returns>
    public Task<bool> PrintAsync(PrintOptions options) => this.InvokeAsync<bool>(options);

    /// <summary>
    /// Prints window's web page.
    /// </summary>
    /// <returns>success</returns>
    public Task<bool> PrintAsync() => this.InvokeAsync<bool>(string.Empty);

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
        var tcs = new TaskCompletionSource<bool>();

        BridgeConnector.Socket.Once<bool>("webContents-printToPDF-completed", tcs.SetResult);

        if (options == null)
        {
            BridgeConnector.Socket.Emit("webContents-printToPDF", Id, "", path);
        }
        else
        {
            BridgeConnector.Socket.Emit("webContents-printToPDF", Id, options, path);
        }

        return tcs.Task;
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
    public Task<T> ExecuteJavaScriptAsync<T>(string code, bool userGesture = false)
    {
        var tcs = new TaskCompletionSource<T>();

        BridgeConnector.Socket.Once<T>("webContents-executeJavaScript-completed", tcs.SetResult);
        BridgeConnector.Socket.Emit("webContents-executeJavaScript", Id, code, userGesture);

        return tcs.Task;
    }

    /// <summary>
    /// Is used to get the Url of the loaded page.
    /// It's usefull if a web-server redirects you and you need to know where it redirects. For instance, It's useful in case of Implicit Authorization.
    /// </summary>
    /// <returns>URL of the loaded page</returns>
    public Task<string> GetUrl()
    {
        var tcs = new TaskCompletionSource<string>();

        BridgeConnector.Socket.Once<string>("webContents-getUrl" + Id, tcs.SetResult);
        BridgeConnector.Socket.Emit("webContents-getUrl", Id);

        return tcs.Task;
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
        var tcs = new TaskCompletionSource();

        BridgeConnector.Socket.Once("webContents-loadURL-complete" + Id, () =>
        {
            BridgeConnector.Socket.Off("webContents-loadURL-error" + Id);
            tcs.SetResult();
        });

        BridgeConnector.Socket.Once<string>("webContents-loadURL-error" + Id, (error) => { tcs.SetException(new InvalidOperationException(error)); });

        BridgeConnector.Socket.Emit("webContents-loadURL", Id, url, options);

        return tcs.Task;
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
}