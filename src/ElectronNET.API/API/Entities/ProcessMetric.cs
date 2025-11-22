using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Process metrics information.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class ProcessMetric
    {
        /// <summary>
        /// Gets or sets the process id of the process.
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// Gets or sets the process type. One of: Browser | Tab | Utility | Zygote | Sandbox helper | GPU | Pepper Plugin | Pepper Plugin Broker | Unknown.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the CPU usage of the process.
        /// </summary>
        public CPUUsage Cpu { get; set; }

        /// <summary>
        /// Gets or sets the creation time for this process, represented as the number of milliseconds since the UNIX epoch. Since the pid can be reused after a process dies, use both pid and creationTime to uniquely identify a process.
        /// </summary>
        public double CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the memory information for the process.
        /// </summary>
        public MemoryInfo Memory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the process is sandboxed on OS level.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public bool Sandboxed { get; set; }

        /// <summary>
        /// Gets or sets the integrity level. One of: untrusted | low | medium | high | unknown.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string IntegrityLevel { get; set; }

        /// <summary>
        /// Gets or sets the name of the process. Examples for utility: Audio Service, Content Decryption Module Service, Network Service, Video Capture, etc.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the non-localized name of the process.
        /// </summary>
        public string ServiceName { get; set; }
    }
}