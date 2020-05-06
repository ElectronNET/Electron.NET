namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ResizeOptions
    {
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// good, better, or best. Default is "best";
        /// </summary>
        public string Quality { get; set; } = "best";
    }
}
