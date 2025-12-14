namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class ProgressInfo
    {
        /// <summary>Gets or sets the progress.</summary>
        public string Progress { get; set; }

        /// <summary>
        /// Gets or sets bytes processed per second.
        /// </summary>
        public long BytesPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the percentage completed (0–100).
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes to download.
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes transferred so far.
        /// </summary>
        public long Transferred { get; set; }
    }
}