using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

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
        public int Width { get; set; }

        /// <summary>
        /// Window's height in pixels. Default is 600.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// ( if y is used) Window's left offset from screen. Default is to center the
        /// window.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// ( if x is used) Window's top offset from screen. Default is to center the
        /// window.
        /// </summary>
        public int Y { get; set; }

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
        [DefaultValue(true)]
        public bool Movable { get; set; } = true;

        /// <summary>
        /// Whether window is minimizable. This is not implemented on Linux. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Minimizable { get; set; } = true;

        /// <summary>
        /// Whether window is maximizable. This is not implemented on Linux. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Maximizable { get; set; } = true;

        /// <summary>
        /// Whether window is closable. This is not implemented on Linux. Default is true.
        /// </summary>
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
        /// maximize/zoom button should toggle full screen mode or maximize window.Default
        /// is true.
        /// </summary>
        public bool Fullscreenable { get; set; }

        /// <summary>
        /// Whether to show the window in taskbar. Default is false.
        /// </summary>
        public bool SkipTaskbar { get; set; }

        /// <summary>
        /// The kiosk mode. Default is false.
        /// </summary>
        public bool Kiosk { get; set; }

        /// <summary>
        /// Default window title. Default is "Electron.NET".
        /// </summary>
        public string Title { get; set; } = "Electron.NET";

        /// <summary>
        /// The window icon. On Windows it is recommended to use ICO icons to get best
        /// visual effects, you can also leave it undefined so the executable's icon will be used.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Whether window should be shown when created. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Show { get; set; } = true;

        /// <summary>
        /// Specify false to create a . Default is true.
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
        public bool AcceptFirstMouse { get; set; }

        /// <summary>
        /// Whether to hide cursor when typing. Default is false.
        /// </summary>
        public bool DisableAutoHideCursor { get; set; }

        /// <summary>
        /// Auto hide the menu bar unless the Alt key is pressed. Default is false.
        /// </summary>
        public bool AutoHideMenuBar { get; set; }

        /// <summary>
        /// Enable the window to be resized larger than screen. Default is false.
        /// </summary>
        public bool EnableLargerThanScreen { get; set; }

        /// <summary>
        /// Window's background color as Hexadecimal value, like #66CD00 or #FFF or
        /// #80FFFFFF (alpha is supported). Default is #FFF (white).
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Whether window should have a shadow. This is only implemented on macOS. Default
        /// is true.
        /// </summary>
        public bool HasShadow { get; set; }

        /// <summary>
        /// Forces using dark theme for the window, only works on some GTK+3 desktop
        /// environments.Default is false.
        /// </summary>
        public bool DarkTheme { get; set; }

        /// <summary>
        /// Makes the window . Default is false.
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
        [JsonConverter(typeof(StringEnumConverter))]
        public TitleBarStyle TitleBarStyle { get; set; }

        /// <summary>
        /// Shows the title in the tile bar in full screen mode on macOS for all
        /// titleBarStyle options.Default is false.
        /// </summary>
        public bool FullscreenWindowTitle { get; set; }

        /// <summary>
        /// Use WS_THICKFRAME style for frameless windows on Windows, which adds standard
        /// window frame.Setting it to false will remove window shadow and window
        /// animations. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool ThickFrame { get; set; } = true;

        /// <summary>
        /// Add a type of vibrancy effect to the window, only on macOS. Can be
        /// appearance-based, light, dark, titlebar, selection, menu, popover, sidebar,
        /// medium-light or ultra-dark.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Vibrancy Vibrancy { get; set; }

        /// <summary>
        /// Controls the behavior on macOS when option-clicking the green stoplight button
        /// on the toolbar or by clicking the Window > Zoom menu item.If true, the window
        /// will grow to the preferred width of the web page when zoomed, false will cause
        /// it to zoom to the width of the screen.This will also affect the behavior when
        /// calling maximize() directly.Default is false.
        /// </summary>
        public bool ZoomToPageWidth { get; set; }

        /// <summary>
        /// Tab group name, allows opening the window as a native tab on macOS 10.12+.
        /// Windows with the same tabbing identifier will be grouped together.This also
        /// adds a native new tab button to your window's tab bar and allows your app and
        /// window to receive the new-window-for-tab event.
        /// </summary>
        public string TabbingIdentifier { get; set; }

        /// <summary>
        /// Settings of web page's features.
        /// </summary>
        public WebPreferences WebPreferences { get; set; }
    }
}
