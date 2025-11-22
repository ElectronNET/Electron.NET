namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class UpdateInfo
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the files included in this update.
        /// </summary>
        public UpdateFileInfo[] Files { get; set; } = new UpdateFileInfo[0];

        /// <summary>
        /// Gets or sets the release name.
        /// </summary>
        public string ReleaseName { get; set; }

        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
        public ReleaseNoteInfo[] ReleaseNotes { get; set; } = new ReleaseNoteInfo[0];

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the staged rollout percentage, 0-100.
        /// </summary>
        public double StagingPercentage { get; set; }
    }
}