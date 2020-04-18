namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Printer info
    /// </summary>
    public class PrinterInfo
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Is default
        /// </summary>
        public bool IsDefault { get; set; }

    }
}