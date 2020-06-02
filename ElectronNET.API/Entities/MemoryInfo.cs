namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryInfo
    {
        /// <summary>
        /// The amount of memory currently pinned to actual physical RAM.
        /// </summary>
        public int WorkingSetSize { get; set; }

        /// <summary>
        /// The maximum amount of memory that has ever been pinned to actual physical RAM. 
        /// </summary>
        public int PeakWorkingSetSize { get; set; }

        /// <summary>
        /// The amount of memory not shared by other processes, such as JS heap or HTML
        /// content.
        /// </summary>
        public int PrivateBytes { get; set; }
    }
}