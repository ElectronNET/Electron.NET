namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginSettings
    {
        /// <summary>
        /// true to open the app at login, false to remove the app as a login item. Defaults
        /// to false.
        /// </summary>
        public bool OpenAtLogin { get; set; }

        /// <summary>
        /// true to open the app as hidden. Defaults to false. The user can edit this
        /// setting from the System Preferences so
        /// app.getLoginItemStatus().wasOpenedAsHidden should be checked when the app is
        /// opened to know the current value.This setting is only supported on macOS.
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