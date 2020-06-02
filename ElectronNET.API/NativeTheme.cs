using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;

namespace ElectronNET.API
{
    /// <summary>
    /// Read and respond to changes in Chromium's native color theme.
    /// </summary>
    public sealed class NativeTheme
    {
        private static NativeTheme _nativeTheme;
        private static object _syncRoot = new object();

        internal NativeTheme() { }

        internal static NativeTheme Instance
        {
            get
            {
                if (_nativeTheme == null)
                {
                    lock (_syncRoot)
                    {
                        if (_nativeTheme == null)
                        {
                            _nativeTheme = new NativeTheme();
                        }
                    }
                }

                return _nativeTheme;
            }
        }

        /// <summary>
        /// Setting this property to <see cref="ThemeSourceMode.System"/> will remove the override and everything will be reset to the OS default. By default 'ThemeSource' is <see cref="ThemeSourceMode.System"/>.
        /// <para/>
        /// Settings this property to <see cref="ThemeSourceMode.Dark"/> will have the following effects:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="ShouldUseDarkColorsAsync"/> will be <see langword="true"/> when accessed</description>
        /// </item>
        /// <item>
        /// <description>Any UI Electron renders on Linux and Windows including context menus, devtools, etc. will use the dark UI.</description>
        /// </item>
        /// <item>
        /// <description>Any UI the OS renders on macOS including menus, window frames, etc. will use the dark UI.</description>
        /// </item>
        /// <item>
        /// <description>The 'prefers-color-scheme' CSS query will match 'dark' mode.</description>
        /// </item>
        /// <item>
        /// <description>The 'updated' event will be emitted</description>
        /// </item> 
        /// </list>
        /// <para/>
        /// Settings this property to <see cref="ThemeSourceMode.Light"/> will have the following effects:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="ShouldUseDarkColorsAsync"/> will be <see langword="false"/> when accessed</description>
        /// </item>
        /// <item>
        /// <description>Any UI Electron renders on Linux and Windows including context menus, devtools, etc. will use the light UI.</description>
        /// </item>
        /// <item>
        /// <description>Any UI the OS renders on macOS including menus, window frames, etc. will use the light UI.</description>
        /// </item>
        /// <item>
        /// <description>The 'prefers-color-scheme' CSS query will match 'light' mode.</description>
        /// </item>
        /// <item>
        /// <description>The 'updated' event will be emitted</description>
        /// </item>
        /// </list> 
        /// The usage of this property should align with a classic "dark mode" state machine in your application where the user has three options.
        /// <para/>
        /// <list type="bullet">
        /// <item>
        /// <description>Follow OS: SetThemeSource(ThemeSourceMode.System);</description>
        /// </item>
        /// <item>
        /// <description>Dark Mode: SetThemeSource(ThemeSourceMode.Dark);</description>
        /// </item>
        /// <item>
        /// <description>Light Mode: SetThemeSource(ThemeSourceMode.Light);</description>
        /// </item>
        /// </list>
        /// Your application should then always use <see cref="ShouldUseDarkColorsAsync"/> to determine what CSS to apply.
        /// </summary>
        /// <param name="themeSourceMode">The new ThemeSource.</param>
        public void SetThemeSource(ThemeSourceMode themeSourceMode)
        {
            var themeSource = themeSourceMode.GetDescription();

            BridgeConnector.Socket.Emit("nativeTheme-themeSource", themeSource);
        }

        /// <summary>
        /// A <see cref="ThemeSourceMode"/> property that can be <see cref="ThemeSourceMode.System"/>, <see cref="ThemeSourceMode.Light"/> or <see cref="ThemeSourceMode.Dark"/>. It is used to override (<seealso cref="SetThemeSource"/>) and
        /// supercede the value that Chromium has chosen to use internally.
        /// </summary>
        public Task<ThemeSourceMode> GetThemeSourceAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ThemeSourceMode>();

            BridgeConnector.Socket.On("nativeTheme-themeSource-getCompleted", (themeSource) =>
            {
                BridgeConnector.Socket.Off("nativeTheme-themeSource-getCompleted");

                var themeSourceValue = (ThemeSourceMode)Enum.Parse(typeof(ThemeSourceMode), (string)themeSource, true);

                taskCompletionSource.SetResult(themeSourceValue);
            });

            BridgeConnector.Socket.Emit("nativeTheme-themeSource-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// A <see cref="bool"/> for if the OS / Chromium currently has a dark mode enabled or is
        /// being instructed to show a dark-style UI. If you want to modify this value you
        /// should use <see cref="SetThemeSource"/>.
        /// </summary>
        public Task<bool> ShouldUseDarkColorsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("nativeTheme-shouldUseDarkColors-completed", (shouldUseDarkColors) => {
                BridgeConnector.Socket.Off("nativeTheme-shouldUseDarkColors-completed");

                taskCompletionSource.SetResult((bool)shouldUseDarkColors);
            });

            BridgeConnector.Socket.Emit("nativeTheme-shouldUseDarkColors");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// A <see cref="bool"/> for if the OS / Chromium currently has high-contrast mode enabled or is
        /// being instructed to show a high-contrast UI.
        /// </summary>
        public Task<bool> ShouldUseHighContrastColorsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("nativeTheme-shouldUseHighContrastColors-completed", (shouldUseHighContrastColors) => {
                BridgeConnector.Socket.Off("nativeTheme-shouldUseHighContrastColors-completed");

                taskCompletionSource.SetResult((bool)shouldUseHighContrastColors);
            });

            BridgeConnector.Socket.Emit("nativeTheme-shouldUseHighContrastColors");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// A <see cref="bool"/> for if the OS / Chromium currently has an inverted color scheme or is
        /// being instructed to use an inverted color scheme.
        /// </summary>
        public Task<bool> ShouldUseInvertedColorSchemeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("nativeTheme-shouldUseInvertedColorScheme-completed", (shouldUseInvertedColorScheme) => {
                BridgeConnector.Socket.Off("nativeTheme-shouldUseInvertedColorScheme-completed");

                taskCompletionSource.SetResult((bool)shouldUseInvertedColorScheme);
            });

            BridgeConnector.Socket.Emit("nativeTheme-shouldUseInvertedColorScheme");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Emitted when something in the underlying NativeTheme has changed. This normally means that either the value of <see cref="ShouldUseDarkColorsAsync"/>,
        /// <see cref="ShouldUseHighContrastColorsAsync"/> or <see cref="ShouldUseInvertedColorSchemeAsync"/> has changed. You will have to check them to determine which one has changed.
        /// </summary>
        public event Action Updated
        {
            add
            {
                if (_updated == null)
                {
                    BridgeConnector.Socket.On("nativeTheme-updated" + GetHashCode(), () =>
                    {
                        _updated();
                    });

                    BridgeConnector.Socket.Emit("register-nativeTheme-updated-event", GetHashCode());
                }
                _updated += value;
            }
            remove
            {
                _updated -= value;

                if (_updated == null)
                {
                    BridgeConnector.Socket.Off("nativeTheme-updated" + GetHashCode());
                }
            }
        }

        private event Action _updated;
    }
}