namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CreateFromBufferOptions
    {
        /// <summary>
        /// Gets or sets the width. Required for bitmap buffers passed to nativeImage.createFromBuffer.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height. Required for bitmap buffers passed to nativeImage.createFromBuffer.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// The image scale factor. Optional, defaults to 1.0.
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;
    }
}