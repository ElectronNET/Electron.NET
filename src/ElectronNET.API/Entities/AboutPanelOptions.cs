namespace ElectronNET.API.Entities
{
    /// <summary>
    /// About panel options.
    /// </summary>
    public class AboutPanelOptions
    {
        /// <summary>
        /// The app's name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The app's version.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Copyright information.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The app's build version number.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Credit information.
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// List of app authors.
        /// </summary>
        public string[] Authors { get; set; }

        /// <summary>
        /// The app's website.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Path to the app's icon. On Linux, will be shown as 64x64 pixels while retaining aspect ratio.
        /// </summary>
        public string IconPath { get; set; }
    }
}