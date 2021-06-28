using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class WebPreferences
    {
        /// <summary>
        /// Whether to enable DevTools. If it is set to false, can not use
        /// BrowserWindow.webContents.openDevTools() to open DevTools.Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool DevTools { get; set; } = true;

        /// <summary>
        /// Whether node integration is enabled. Required to enable IPC. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool NodeIntegration { get; set; } = true;

        /// <summary>
        /// Whether node integration is enabled in web workers. Default is false.
        /// </summary>
        public bool NodeIntegrationInWorker { get; set; }

        /// <summary>
        /// Specifies a script that will be loaded before other scripts run in the page.
        /// This script will always have access to node APIs no matter whether node
        /// integration is turned on or off.The value should be the absolute file path to
        /// the script. When node integration is turned off, the preload script can
        /// reintroduce Node global symbols back to the global scope.
        /// </summary>
        public string Preload { get; set; }

        /// <summary>
        /// If set, this will sandbox the renderer associated with the window, making it
        /// compatible with the Chromium OS-level sandbox and disabling the Node.js engine.
        /// This is not the same as the nodeIntegration option and the APIs available to the
        /// preload script are more limited. Read more about the option.This option is
        /// currently experimental and may change or be removed in future Electron releases.
        /// </summary>
        public bool Sandbox { get; set; }

        /// <summary>
        /// Sets the session used by the page according to the session's partition string.
        /// If partition starts with persist:, the page will use a persistent session
        /// available to all pages in the app with the same partition.If there is no
        /// persist: prefix, the page will use an in-memory session. By assigning the same
        /// partition, multiple pages can share the same session.Default is the default
        /// session.
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// The default zoom factor of the page, 3.0 represents 300%. Default is 1.0.
        /// </summary>
        public int ZoomFactor { get; set; }

        /// <summary>
        /// Enables JavaScript support. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Javascript { get; set; } = true;

        /// <summary>
        /// When false, it will disable the same-origin policy (usually using testing
        /// websites by people), and set allowRunningInsecureContent to true if this options
        /// has not been set by user. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool WebSecurity { get; set; } = true;

        /// <summary>
        /// Allow an https page to run JavaScript, CSS or plugins from http URLs. Default is
        /// false.
        /// </summary>
        public bool AllowRunningInsecureContent { get; set; }

        /// <summary>
        /// Enables image support. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Images { get; set; } = true;

        /// <summary>
        /// Make TextArea elements resizable. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool TextAreasAreResizable { get; set; } = true;

        /// <summary>
        /// Enables WebGL support. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Webgl { get; set; } = true;

        /// <summary>
        /// Enables WebAudio support. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Webaudio { get; set; } = true;

        /// <summary>
        /// Whether plugins should be enabled. Default is false.
        /// </summary>
        public bool Plugins { get; set; }

        /// <summary>
        /// Enables Chromium's experimental features. Default is false.
        /// </summary>
        public bool ExperimentalFeatures { get; set; }

        /// <summary>
        /// Enables Chromium's experimental canvas features. Default is false.
        /// </summary>
        public bool ExperimentalCanvasFeatures { get; set; }

        /// <summary>
        /// Enables scroll bounce (rubber banding) effect on macOS. Default is false.
        /// </summary>
        public bool ScrollBounce { get; set; }

        /// <summary>
        /// A list of feature strings separated by ,, like CSSVariables,KeyboardEventKey to
        /// enable.The full list of supported feature strings can be found in the file.
        /// </summary>
        public string EnableBlinkFeatures { get; set; }

        /// <summary>
        /// A list of feature strings separated by ,, like CSSVariables,KeyboardEventKey to
        /// disable.The full list of supported feature strings can be found in the file.
        /// </summary>
        public string DisableBlinkFeatures { get; set; }

        /// <summary>
        /// Sets the default font for the font-family.
        /// </summary>
        public DefaultFontFamily DefaultFontFamily { get; set; }

        /// <summary>
        /// Defaults to 16.
        /// </summary>
        public int DefaultFontSize { get; set; } = 16;

        /// <summary>
        /// Defaults to 13.
        /// </summary>
        public int DefaultMonospaceFontSize { get; set; }

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public int MinimumFontSize { get; set; }

        /// <summary>
        /// Defaults to ISO-8859-1.
        /// </summary>
        public string DefaultEncoding { get; set; }

        /// <summary>
        /// Whether to throttle animations and timers when the page becomes background. This
        /// also affects the[Page Visibility API][#page-visibility]. Defaults to true.
        /// </summary>
        [DefaultValue(true)]
        public bool BackgroundThrottling { get; set; } = true;

        /// <summary>
        /// Whether to enable offscreen rendering for the browser window. Defaults to false.
        /// </summary>
        public bool Offscreen { get; set; }

        /// <summary>
        /// Whether to run Electron APIs and the specified preload script in a separate
        /// JavaScript context. Defaults to false. The context that the preload script runs
        /// in will still have full access to the document and window globals but it will
        /// use its own set of JavaScript builtins (Array, Object, JSON, etc.) and will be
        /// isolated from any changes made to the global environment by the loaded page.The
        /// Electron API will only be available in the preload script and not the loaded
        /// page. This option should be used when loading potentially untrusted remote
        /// content to ensure the loaded content cannot tamper with the preload script and
        /// any Electron APIs being used. This option uses the same technique used by . You
        /// can access this context in the dev tools by selecting the 'Electron Isolated
        /// Context' entry in the combo box at the top of the Console tab. This option is
        /// currently experimental and may change or be removed in future Electron releases.
        /// </summary>
        [DefaultValue(true)]
        public bool ContextIsolation { get; set; } = true;

        /// <summary>
        /// Whether to use native window.open(). Defaults to false. This option is currently experimental.
        /// </summary>
        public bool NativeWindowOpen { get; set; }

        /// <summary>
        /// Whether to enable the Webview. Defaults to the value of the nodeIntegration option. The
        /// preload script configured for the Webview will have node integration enabled
        /// when it is executed so you should ensure remote/untrusted content is not able to
        /// create a Webview tag with a possibly malicious preload script.You can use the
        /// will-attach-webview event on to strip away the preload script and to validate or
        /// alter the Webview's initial settings.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [webview tag]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool WebviewTag { get; set; } = false;

        /// <summary>
        /// Whether to enable the remote module. Defaults to false.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableRemoteModule { get; set; } = false;
    }
}