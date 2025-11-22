namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CreateFromBitmapOptions
    {
        /// <summary>
        /// Gets or sets the width in pixels. Required for nativeImage.createFromBitmap(buffer, options).
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels. Required for nativeImage.createFromBitmap(buffer, options).
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the image scale factor. Optional, defaults to 1.0.
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;
    }
}