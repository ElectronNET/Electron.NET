using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use ImageOptions instead.")]
    public class BitmapOptions
    {
        /// <summary>
        /// Gets or sets the scale factor
        /// </summary>
        public float ScaleFactor { get; set; } = NativeImage.DefaultScaleFactor;
        /// <summary>
        /// Utility conversion for obsolete class
        /// </summary>
        public static implicit operator ImageOptions(BitmapOptions o) => new() {ScaleFactor = o.ScaleFactor};
    }
}
