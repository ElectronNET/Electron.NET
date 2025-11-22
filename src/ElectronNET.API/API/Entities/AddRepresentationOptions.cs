using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class AddRepresentationOptions
    {
        /// <summary>
        /// Gets or sets the width in pixels. Defaults to 0. Required if a bitmap buffer is specified as <see cref="Buffer"/>.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels. Defaults to 0. Required if a bitmap buffer is specified as <see cref="Buffer"/>.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the image scale factor. Defaults to 1.0.
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the buffer containing the raw image data.
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets or sets the data URL containing a base 64 encoded PNG or JPEG image.
        /// </summary>
        public string DataUrl { get; set; }
    }
}