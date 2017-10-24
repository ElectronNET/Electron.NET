namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginItemSettings
    {
        /// <summary>
        /// true if the app is set to open at login.
        /// </summary>
        public bool OpenAtLogin { get; set; }

        /// <summary>
        /// true if the app is set to open as hidden at login. This setting is only 
        /// supported on macOS.
        /// </summary>
        public bool OpenAsHidden { get; set; }

        /// <summary>
        /// true if the app was opened at login automatically. This setting is only 
        /// supported on macOS.
        /// </summary>
        public bool WasOpenedAtLogin { get; set; }

        /// <summary>
        /// true if the app was opened as a hidden login item. This indicates that the app 
        /// should not open any windows at startup.This setting is only supported on macOS.
        /// </summary>
        public bool WasOpenedAsHidden { get; set; }

        /// <summary>
        /// true if the app was opened as a login item that should restore the state from
        /// the previous session.This indicates that the app should restore the windows
        /// that were open the last time the app was closed.This setting is only supported
        /// on macOS.
        /// </summary>
        public bool RestoreState { get; set; }
    }
}