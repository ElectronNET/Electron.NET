using ElectronNET.Converter;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BrowserWindowOptions
    {
        /// <summary>
        /// Window's width in pixels. Default is 800.
        /// </summary>
        public int Width { get; set; } = 800;

        /// <summary>
        /// Window's height in pixels. Default is 600.
        /// </summary>
        public int Height { get; set; } = 600;

        /// <summary>
        /// ( if y is used) Window's left offset from screen. Default is to center the
        /// window.
        /// </summary>
        public int X { get; set; } = -1;

        /// <summary>
        /// ( if x is used) Window's top offset from screen. Default is to center the
        /// window.
        /// </summary>
        public int Y { get; set; } = -1;

        /// <summary>
        /// The width and height would be used as web page's size, which means the actual
        /// window's size will include window frame's size and be slightly larger. Default
        /// is false.
        /// </summary>
        public bool UseContentSize { get; set; }

        /// <summary>
        /// Show window in the center of the screen.
        /// </summary>
        public bool Center { get; set; }

        /// <summary>
        /// Window's minimum width. Default is 0.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Window's minimum height. Default is 0.
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// Window's maximum width. Default is no limit.
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Window's maximum height. Default is no limit.
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// Whether window is resizable. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Resizable { get; set; } = true;

        /// <summary>
        /// Whether window is movable. This is not implemented on Linux. Default is true.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool Movable { get; set; } = true;

        /// <summary>
        /// Whether window is minimizable. This is not implemented on Linux. Default is true.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool Minimizable { get; set; } = true;

        /// <summary>
        /// Whether window is maximizable. This is not implemented on Linux. Default is true.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool Maximizable { get; set; } = true;

        /// <summary>
        /// Whether window is closable. This is not implemented on Linux. Default is true.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool Closable { get; set; } = true;

        /// <summary>
        /// Whether the window can be focused. Default is true. On Windows setting
        /// focusable: false also implies setting skipTaskbar: true. On Linux setting
        /// focusable: false makes the window stop interacting with wm, so the window will
        /// always stay on top in all workspaces.
        /// </summary>
        [DefaultValue(true)]
        public bool Focusable { get; set; } = true;

        /// <summary>
        /// Whether the window should always stay on top of other windows. Default is false.
        /// </summary>
        public bool AlwaysOnTop { get; set; }

        /// <summary>
        /// Whether the window should show in fullscreen. When explicitly set to false the
        /// fullscreen button will be hidden or disabled on macOS.Default is false.
        /// </summary>
        public bool Fullscreen { get; set; }

        /// <summary>
        /// Whether the window can be put into fullscreen mode. On macOS, also whether the
        /// maximize/zoom button should toggle full screen mode or maximize window. Default
        /// is true (Electron default).
        /// </summary>
        [DefaultValue(true)]
        public bool Fullscreenable { get; set; } = true; // FIX: previously defaulted to false in C#

        /// <summary>
        /// Whether to show the window in taskbar. Default is false.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public bool SkipTaskbar { get; set; }

        /// <summary>
        /// Determines if Blazor is used. Will disable "module" and "process" globals. Default is false.
        /// </summary>
        public bool IsRunningBlazor { get; set; }

        /// <summary>
        /// The kiosk mode. Default is false.
        /// </summary>
        public bool Kiosk { get; set; }

        /// <summary>
        /// Default window title. Default is "Electron.NET".
        /// </summary>
        public string Title { get; set; } = "Electron.NET";

        /// <summary>
        /// The window icon. Can be a NativeImage or a string path. On Windows it is recommended to use ICO icons; when undefined, the executable's icon will be used.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Whether window should be shown when created. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Show { get; set; } = true;

        /// <summary>
        /// Specify false to create a frameless window. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Frame { get; set; } = true;

        /// <summary>
        /// Whether this is a modal window. This only works when the window is a child
        /// window.Default is false.
        /// </summary>
        public bool Modal { get; set; }

        /// <summary>
        /// Whether the web view accepts a single mouse-down event that simultaneously
        /// activates the window.Default is false.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool AcceptFirstMouse { get; set; }

        /// <summary>
        /// Whether to hide cursor when typing. Default is false.
        /// </summary>
        public bool DisableAutoHideCursor { get; set; }

        /// <summary>
        /// Auto hide the menu bar unless the Alt key is pressed. Default is false.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public bool AutoHideMenuBar { get; set; }

        /// <summary>
        /// Enable the window to be resized larger than screen. Default is false.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool EnableLargerThanScreen { get; set; }

        /// <summary>
        /// The window's background color in Hex, RGB, RGBA, HSL, HSLA or named CSS color format. Alpha in #AARRGGBB format is supported if transparent is set to true. Default is #FFF (white).
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Initial opacity of the window, between 0.0 (fully transparent) and 1.0 (fully opaque). Only implemented on Windows and macOS.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public double? Opacity { get; set; }

        /// <summary>
        /// Whether window should have a shadow. Default is true.
        /// </summary>
        public bool HasShadow { get; set; }

        /// <summary>
        /// Forces using dark theme for the window, only works on some GTK+3 desktop environments. Default is false.
        /// </summary>
        public bool DarkTheme { get; set; }

        /// <summary>
        /// Makes the window transparent. Default is false.
        /// </summary>
        public bool Transparent { get; set; }

        /// <summary>
        /// The type of window, default is normal window.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The style of window title bar. Default is default. Possible values are:
        /// 'default' | 'hidden' | 'hiddenInset' | 'customButtonsOnHover'
        /// </summary>
        public TitleBarStyle TitleBarStyle { get; set; }

        /// <summary>
        /// Set a custom position for the traffic light buttons in frameless windows (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        public Point TrafficLightPosition { get; set; }

        /// <summary>
        /// Configures the window's title bar overlay when using a frameless window.
        /// Can be either:
        /// - false: No title bar overlay.
        /// - true: Enables the default title bar overlay.
        /// - An object defining custom overlay options (such as height, color, etc.).
        /// 
        /// Default is false.
        /// </summary>
        [JsonConverter(typeof(TitleBarOverlayConverter))]
        public TitleBarOverlay TitleBarOverlay { get; set; }

        /// <summary>
        /// Shows the title in the title bar in full screen mode on macOS for all titleBarStyle options. Default is false.
        /// </summary>
        /// <remarks>Not documented by MCP base-window-options / browser-window-options.</remarks>
        public bool FullscreenWindowTitle { get; set; }

        /// <summary>
        /// Use WS_THICKFRAME style for frameless windows on Windows, which adds standard
        /// window frame.Setting it to false will remove window shadow and window
        /// animations. Default is true.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool ThickFrame { get; set; } = true;

        /// <summary>
        /// Whether frameless window should have rounded corners. Default is true. Setting this
        /// property to false will prevent the window from being fullscreenable on macOS. On
        /// Windows versions older than Windows 11 Build 22000 this property has no effect, and
        /// frameless windows will not have rounded corners.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        [DefaultValue(true)]
        public bool RoundedCorners { get; set; } = true;

        /// <summary>
        /// Add a type of vibrancy effect to the window, only on macOS. Can be
        /// appearance-based, titlebar, selection, menu, popover, sidebar, header, sheet,
        /// window, hud, fullscreen-ui, tooltip, content, under-window, or under-page.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public Vibrancy Vibrancy { get; set; }

        /// <summary>
        /// Controls the behavior on macOS when option-clicking the green stoplight button
        /// on the toolbar or by clicking the Window > Zoom menu item.If true, the window
        /// will grow to the preferred width of the web page when zoomed, false will cause
        /// it to zoom to the width of the screen.This will also affect the behavior when
        /// calling maximize() directly.Default is false.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool ZoomToPageWidth { get; set; }

        /// <summary>
        /// Tab group name, allows opening the window as a native tab on macOS 10.12+.
        /// Windows with the same tabbing identifier will be grouped together.This also
        /// adds a native new tab button to your window's tab bar and allows your app and
        /// window to receive the new-window-for-tab event.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string TabbingIdentifier { get; set; }

        /// <summary>
        /// Settings of web page's features.
        /// </summary>
        public WebPreferences WebPreferences { get; set; }

        /// <summary>
        /// A proxy to set on creation in the format host:port.
        /// The proxy can be alternatively set using the BrowserWindow.WebContents.SetProxyAsync function.
        /// </summary>
        /// <remarks>Not documented by MCP base-window-options / browser-window-options.</remarks>
        public string Proxy { get; set; }

        /// <summary>
        /// The credentials of the Proxy in the format username:password.
        /// These will only be used if the Proxy field is also set.
        /// </summary>
        /// <remarks>Not documented by MCP base-window-options / browser-window-options.</remarks>
        public string ProxyCredentials { get; set; }

        /// <summary>
        /// Gets or sets whether to use pre-Lion fullscreen on macOS. Default is false.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool SimpleFullscreen { get; set; }

        /// <summary>
        /// Gets or sets whether the window should be hidden when the user toggles into mission control (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool HiddenInMissionControl { get; set; }

        /// <summary>
        /// Gets or sets how the material appearance should reflect window activity state on macOS. Must be used with the vibrancy property.
        /// Possible values: 'followWindow' (default), 'active', 'inactive'.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string VisualEffectState { get; set; }

        /// <summary>
        /// Gets or sets the system-drawn background material on Windows. Can be 'auto', 'none', 'mica', 'acrylic' or 'tabbed'.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string BackgroundMaterial { get; set; }
    }
}