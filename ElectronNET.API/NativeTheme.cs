using System;
using System.Threading.Tasks;

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
        /// Checks if the new ThemeSource is valid.
        /// </summary>
        /// <param name="themeSource">The new ThemeSource to check.</param>
        /// <returns>True, if is a valid ThemeSource.</returns>
        internal bool IsValidThemeSource(string themeSource)
        {
            var result =
                string.Equals(themeSource, "dark", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(themeSource, "light", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(themeSource, "system", StringComparison.OrdinalIgnoreCase);

            return result;
        }

        /// <summary>
        /// Setting this property to 'system' will remove the override and everything will be reset to the OS default. By default 'ThemeSource' is 'system'.
        /// <para/>
        /// Settings this property to 'dark' will have the following effects:
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
        /// Settings this property to 'light' will have the following effects:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="ShouldUseDarkColorsAsync"/> will be <see langword="false"/> false when accessed</description>
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
        /// <description>Follow OS: SetThemeSource("system");</description>
        /// </item>
        /// <item>
        /// <description>Dark Mode: SetThemeSource("dark");</description>
        /// </item>
        /// <item>
        /// <description>Light Mode: SetThemeSource("light");</description>
        /// </item>
        /// </list>
        /// Your application should then always use <see cref="ShouldUseDarkColorsAsync"/> to determine what CSS to apply.
        /// </summary>
        /// <param name="themeSource">The new ThemeSource.</param>
        public void SetThemeSource(string themeSource)
        {
            // Check for supported themeSource, otherwise it sets the default
            if (!IsValidThemeSource(themeSource))
            {
                themeSource = "system";
            }

            BridgeConnector.Socket.Emit("nativeTheme-themeSource", themeSource.ToLower());
        }

        /// <summary>
        /// A <see cref="string"/> property that can be 'system', 'light' or 'dark'. It is used to override (<seealso cref="SetThemeSource"/>) and
        /// supercede the value that Chromium has chosen to use internally.
        /// </summary>
        public Task<string> GetThemeSourceAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("nativeTheme-themeSource-getCompleted", (themeSource) =>
            {
                BridgeConnector.Socket.Off("nativeTheme-themeSource-getCompleted");

                taskCompletionSource.SetResult((string)themeSource);
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