namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for NativeImage.resize: optional width/height and quality.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
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
        /// 'good', 'better', or 'best'. Default is 'best'.
        /// </summary>
        public string Quality { get; set; } = "best";
    }
}