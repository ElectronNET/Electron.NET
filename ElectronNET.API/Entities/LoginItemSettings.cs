namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginItemSettings
    {
        /// <summary>
        /// <see langword="true"/> if the app is set to open at login.
        /// </summary>
        public bool OpenAtLogin { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app is set to open as hidden at login. This setting is not available
        /// on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        public bool OpenAsHidden { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened at login automatically. This setting is not available
        /// on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        public bool WasOpenedAtLogin { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened as a hidden login item. This indicates that the app should not
        /// open any windows at startup. This setting is not available on
        /// <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        public bool WasOpenedAsHidden { get; set; }

        /// <summary>
        /// <see langword="true"/> if the app was opened as a login item that should restore the state from the previous
        /// session. This indicates that the app should restore the windows that were open the last time the app was closed.
        /// This setting is not available on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        public bool RestoreState { get; set; }
    }
}