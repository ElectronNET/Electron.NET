namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum PathName
    {
        /// <summary>
        /// User’s home directory.
        /// </summary>
        home,

        /// <summary>
        /// Per-user application data directory.
        /// </summary>
        appData,

        /// <summary>
        /// The directory for storing your app’s configuration files, 
        /// which by default it is the appData directory appended with your app’s name.
        /// </summary>
        userData,

        /// <summary>
        /// Temporary directory.
        /// </summary>
        temp,

        /// <summary>
        /// The current executable file.
        /// </summary>
        exe,

        /// <summary>
        /// The libchromiumcontent library.
        /// </summary>
        module,

        /// <summary>
        /// The current user’s Desktop directory.
        /// </summary>
        desktop,

        /// <summary>
        /// Directory for a user’s “My Documents”.
        /// </summary>
        documents,

        /// <summary>
        /// Directory for a user’s downloads.
        /// </summary>
        downloads,

        /// <summary>
        /// Directory for a user’s music.
        /// </summary>
        music,

        /// <summary>
        /// Directory for a user’s pictures.
        /// </summary>
        pictures,

        /// <summary>
        /// Directory for a user’s videos.
        /// </summary>
        videos,

        /// <summary>
        /// The logs
        /// </summary>
        logs,

        /// <summary>
        /// Full path to the system version of the Pepper Flash plugin.
        /// </summary>
        pepperFlashSystemPlugin
    }
}
