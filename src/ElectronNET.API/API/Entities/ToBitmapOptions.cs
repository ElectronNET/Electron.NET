namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for nativeImage.toBitmap; supports optional scaleFactor (defaults to 1.0) per MCP.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class ToBitmapOptions
    {
        /// <summary>
        /// Gets or sets the image scale factor. Defaults to 1.0.
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;
    }
}