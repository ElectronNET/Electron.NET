namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
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
        /// opened to know the current value. This setting is not available on <see href="https://www.electronjs.org/docs/tutorial/mac-app-store-submission-guide">MAS builds</see>.
        /// </summary>
        public bool OpenAsHidden { get; set; }

        /// <summary>
        /// The executable to launch at login. Defaults to process.execPath.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The command-line arguments to pass to the executable. Defaults to an empty
        /// array.Take care to wrap paths in quotes.
        /// </summary>
        public string[] Args { get; set; }
    }
}