namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// The version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UpdateFileInfo[] Files { get; set; } = new UpdateFileInfo[0];

        /// <summary>
        /// The release name.
        /// </summary>
        public string ReleaseName { get; set; }

        /// <summary>
        /// The release notes.
        /// </summary>
        public ReleaseNoteInfo[] ReleaseNotes { get; set; } = new ReleaseNoteInfo[0];

        /// <summary>
        /// 
        /// </summary>
        public string ReleaseDate { get; set; }

        /// <summary>
        /// The staged rollout percentage, 0-100.
        /// </summary>
        public int StagingPercentage { get; set; }
    }
}