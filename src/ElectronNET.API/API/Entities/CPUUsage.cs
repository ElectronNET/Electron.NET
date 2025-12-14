namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CPUUsage
    {
        /// <summary>
        /// Percentage of CPU used since the last call to getCPUUsage. First call returns 0.
        /// </summary>
        public double PercentCPUUsage { get; set; }

        /// <summary>
        /// Total seconds of CPU time used since process startup, if available.
        /// </summary>
        public double? CumulativeCPUUsage { get; set; }

        /// <summary>
        /// The number of average idle CPU wakeups per second since the last call to
        /// getCPUUsage. First call returns 0. Will always return 0 on Windows.
        /// </summary>
        public double IdleWakeupsPerSecond { get; set; }
    }
}