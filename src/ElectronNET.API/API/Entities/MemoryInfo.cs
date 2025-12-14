using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Process memory info as returned by process.getProcessMemoryInfo().
    /// Values are reported in Kilobytes.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class MemoryInfo
    {
        /// <summary>
        /// Gets or sets the amount of memory currently pinned to actual physical RAM.
        /// </summary>
        public int WorkingSetSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of memory that has ever been pinned to actual physical RAM.
        /// </summary>
        public int PeakWorkingSetSize { get; set; }

        /// <summary>
        /// Gets or sets the amount of memory not shared by other processes, such as JS heap or HTML content. Windows only.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public int PrivateBytes { get; set; }
    }
}