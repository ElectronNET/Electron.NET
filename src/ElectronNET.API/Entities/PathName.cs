using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the PathName enumeration.
    /// </summary>
    public enum PathName
    {
        /// <summary>
        /// User’s home directory.
        /// </summary>
        [Description("home")]
        Home,

        /// <summary>
        /// Per-user application data directory.
        /// </summary>
        [Description("appData")]
        AppData,

        /// <summary>
        /// The directory for storing your app’s configuration files, 
        /// which by default it is the appData directory appended with your app’s name.
        /// </summary>
        [Description("userData")]
        UserData,

        /// <summary>
        /// Temporary directory.
        /// </summary>
        [Description("temp")]
        Temp,

        /// <summary>
        /// The current executable file.
        /// </summary>
        [Description("exe")]
        Exe,

        /// <summary>
        /// The libchromiumcontent library.
        /// </summary>
        [Description("Module")]
        Module,

        /// <summary>
        /// The current user’s Desktop directory.
        /// </summary>
        [Description("desktop")]
        Desktop,

        /// <summary>
        /// Directory for a user’s “My Documents”.
        /// </summary>
        [Description("documents")]
        Documents,

        /// <summary>
        /// Directory for a user’s downloads.
        /// </summary>
        [Description("downloads")]
        Downloads,

        /// <summary>
        /// Directory for a user’s music.
        /// </summary>
        [Description("music")]
        Music,

        /// <summary>
        /// Directory for a user’s pictures.
        /// </summary>
        [Description("pictures")]
        Pictures,

        /// <summary>
        /// Directory for a user’s videos.
        /// </summary>
        [Description("videos")]
        Videos,

        /// <summary>
        /// The logs.
        /// </summary>
        [Description("logs")]
        Logs,

        /// <summary>
        /// Full path to the system version of the Pepper Flash plugin.
        /// </summary>
        [Description("PepperFlashSystemPlugin")]
        PepperFlashSystemPlugin
    }
}
