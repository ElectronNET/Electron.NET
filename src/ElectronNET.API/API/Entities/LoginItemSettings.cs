using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Return object for app.getLoginItemSettings() on macOS and Windows.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class LoginItemSettings
    {
        /// <summary>
        /// <see langword="true"/> if the app is set to open at login.
        /// </summary>
        public bool OpenAtLogin { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app is set to open as hidden at login. Deprecated on macOS 13 and up; not available
        /// on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool OpenAsHidden { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened at login automatically. This setting is not available
        /// on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool WasOpenedAtLogin { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened as a hidden login item. This indicates that the app should not
        /// open any windows at startup. Deprecated on macOS 13 and up; not available on
        /// <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool WasOpenedAsHidden { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened as a login item that should restore the state from the previous
        /// session. This indicates that the app should restore the windows that were open the last time the app was closed.
        /// Deprecated on macOS 13 and up; not available on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool RestoreState { get; set; }

        /// <summary>
        /// macOS status: one of <c>not-registered</c>, <c>enabled</c>, <c>requires-approval</c>, or <c>not-found</c>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Status { get; set; }

        /// <summary>
        /// Windows: true if app is set to open at login and its run key is not deactivated.
        /// Differs from <c>OpenAtLogin</c> as it ignores the <c>args</c> option; this is true if the given executable would be launched at login with any arguments.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public bool ExecutableWillLaunchAtLogin { get; set; }

        /// <summary>
        /// Windows launch entries found in registry.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public LoginItemLaunchItem[] LaunchItems { get; set; }
    }
}