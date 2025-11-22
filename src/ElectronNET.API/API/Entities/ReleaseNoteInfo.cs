namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class ReleaseNoteInfo
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        public string Note { get; set; }
    }
}