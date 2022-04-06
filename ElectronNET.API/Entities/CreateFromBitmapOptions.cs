using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use CreateOptions instead")]
    public class CreateFromBitmapOptions
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
        /// Gets or sets the scalefactor
        /// </summary>
        public float ScaleFactor { get; set; } = NativeImage.DefaultScaleFactor;

        /// <summary>
        /// Utility conversion for obsolete class
        /// </summary>
        public static implicit operator CreateOptions(CreateFromBitmapOptions o) => new()
            {Width = o.Width, Height = o.Height, ScaleFactor = o.ScaleFactor};
    }
}
