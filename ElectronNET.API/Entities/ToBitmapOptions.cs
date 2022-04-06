using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use ImageOptions instead.")]
    public class ToBitmapOptions
    {
        /// <summary>
        /// Gets or sets the scalefactor
        /// </summary>
        public float ScaleFactor { get; set; } = NativeImage.DefaultScaleFactor;
        /// <summary>
        /// Utility conversion for obsolete class
        /// </summary>
        public static implicit operator ImageOptions(ToBitmapOptions o) => new () {ScaleFactor = o.ScaleFactor};
    }
}
