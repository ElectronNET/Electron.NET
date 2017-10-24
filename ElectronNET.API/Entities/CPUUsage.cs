namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class CPUUsage
    {
        /// <summary>
        /// The number of average idle cpu wakeups per second since the last call to 
        /// getCPUUsage.First call returns 0.
        /// </summary>
        public int IdleWakeupsPerSecond { get; set; }

        /// <summary>
        /// Percentage of CPU used since the last call to getCPUUsage. First call returns 0.
        /// </summary>
        public int PercentCPUUsage { get; set; }
    }
}
