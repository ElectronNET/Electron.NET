namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class UpdateCheckResult
    {
        /// <summary>
        /// Gets or sets the update information discovered by the check.
        /// </summary>
        public UpdateInfo UpdateInfo { get; set; } = new UpdateInfo();

        /// <summary>
        /// Gets or sets the download artifacts (if provided by the updater).
        /// </summary>
        public string[] Download { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token for the update process.
        /// </summary>
        public UpdateCancellationToken CancellationToken { get; set; }
    }
}