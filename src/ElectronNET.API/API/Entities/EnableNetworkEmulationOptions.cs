namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class EnableNetworkEmulationOptions
    {
        /// <summary>
        /// Whether to emulate network outage. Defaults to false.
        /// </summary>
        public bool Offline { get; set; } = false;

        /// <summary>
        /// RTT in ms. Defaults to 0 which will disable latency throttling.
        /// Electron documents this as a Number (Double).
        /// </summary>
        public double Latency { get; set; }

        /// <summary>
        /// Download rate in Bps. Defaults to 0 which will disable download throttling.
        /// Electron documents this as a Number (Double).
        /// </summary>
        public double DownloadThroughput { get; set; }

        /// <summary>
        /// Upload rate in Bps. Defaults to 0 which will disable upload throttling.
        /// Electron documents this as a Number (Double).
        /// </summary>
        public double UploadThroughput { get; set; }
    }
}