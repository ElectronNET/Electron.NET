namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryInfo
    {
        /// <summary>
        /// The maximum amount of memory that has ever been pinned to actual physical RAM. 
        /// On macOS its value will always be 0.
        /// </summary>
        public int PeakWorkingSetSize { get; set; }

        /// <summary>
        /// Process id of the process.
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// The amount of memory not shared by other processes, such as JS heap or HTML
        /// content.
        /// </summary>
        public int PrivateBytes { get; set; }

        /// <summary>
        /// The amount of memory shared between processes, typically memory consumed by the 
        /// Electron code itself
        /// </summary>
        public int SharedBytes { get; set; }

        /// <summary>
        /// The amount of memory currently pinned to actual physical RAM.
        /// </summary>
        public int WorkingSetSize {get; set; }
    }
}
