namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Process metrics information.
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
        /// Creation time for this process in milliseconds since Unix epoch. Can exceed Int32 range and may contain fractional milliseconds.
        /// </summary>
        public double CreationTime { get; set; }

        /// <summary>
        /// Memory information for the process.
        /// </summary>
        public MemoryInfo Memory { get; set; }

        /// <summary>
        /// Whether the process is sandboxed on OS level.
        /// </summary>
        public bool Sandboxed { get; set; }

        /// <summary>
        /// One of the following values:
        /// untrusted | low | medium | high | unknown
        /// </summary>
        public string IntegrityLevel { get; set; }

        /// <summary>
        /// The name of the process.
        /// Examples for utility: Audio Service, Content Decryption Module Service, Network Service, Video Capture, etc.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The non-localized name of the process.
        /// </summary>
        public string ServiceName { get; set; }
    }
}