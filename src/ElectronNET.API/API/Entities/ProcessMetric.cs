namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessMetric
    {
        /// <summary>
        /// Process id of the process.
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// Process type (Browser or Tab or GPU etc).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// CPU usage of the process.
        /// </summary>
        public CPUUsage Cpu { get; set; }

        /// <summary>
        /// Creation time for this process. The time is represented as number of milliseconds since epoch.
        /// Since the <see cref="PId"/> can be reused after a process dies, it is useful to use both the <see cref="PId"/>
        /// and the <see cref="CreationTime"/> to uniquely identify a process.
        /// </summary>
        public int CreationTime { get; set; }

        /// <summary>
        /// Memory information for the process.
        /// </summary>
        public MemoryInfo Memory { get; set; }

        /// <summary>
        /// Whether the process is sandboxed on OS level.
        /// </summary>
        public bool Sandboxed { get; set; }

        /// <summary>
        ///  One of the following values:
        /// untrusted | low | medium | high | unknown
        /// </summary>
        public string IntegrityLevel { get; set; }
    }
}