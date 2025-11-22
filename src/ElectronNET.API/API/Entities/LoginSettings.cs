using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Settings object for app.setLoginItemSettings() on macOS and Windows.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class LoginSettings
    {
        /// <summary>
        /// <see langword="true"/> to open the app at login, <see langword="false"/> to remove the app as a login item.
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public bool OpenAtLogin { get; set; }

        /// <summary>
        /// <see langword="true"/> to open the app as hidden. Defaults to <see langword="false"/>. The user can edit this
        /// setting from the System Preferences so app.getLoginItemSettings().wasOpenedAsHidden should be checked when the app is
        /// opened to know the current value. This setting is not available on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see> and does not work on macOS 13 and up.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool OpenAsHidden { get; set; }

        /// <summary>
        /// The executable to launch at login. Defaults to process.execPath.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string Path { get; set; }

        /// <summary>
        /// The command-line arguments to pass to the executable. Defaults to an empty
        /// array.Take care to wrap paths in quotes.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string[] Args { get; set; }

        /// <summary>
        /// The type of service to add as a login item. Defaults to 'mainAppService'. Only available on macOS 13 and up.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Type { get; set; }

        /// <summary>
        /// The name of the service. Required if <c>Type</c> is non-default. Only available on macOS 13 and up.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Change the startup approved registry key and enable/disable the app in Task Manager and Windows Settings. Defaults to true.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Value name to write into registry. Defaults to the app's AppUserModelId().
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string Name { get; set; }
    }
}