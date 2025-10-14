namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class EnableNetworkEmulationOptions
    {
        /// <summary>
        /// Whether to emulate network outage. Defaults to false.
        /// </summary>
        public bool Offline { get; set; } = false;

        /// <summary>
        /// RTT in ms. Defaults to 0 which will disable latency throttling.
        /// </summary>
        public int Latency { get; set; }

        /// <summary>
        /// Download rate in Bps. Defaults to 0 which will disable download throttling.
        /// </summary>
        public int DownloadThroughput { get; set; }

        /// <summary>
        /// Upload rate in Bps. Defaults to 0 which will disable upload throttling.
        /// </summary>
        public int UploadThroughput { get; set; }
    }
}
