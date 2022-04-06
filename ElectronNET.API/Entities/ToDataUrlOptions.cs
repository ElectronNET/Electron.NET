using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use ImageOptions instead.")]
    public class ToDataUrlOptions
    {
        /// <summary>
        /// Gets or sets the scalefactor
        /// </summary>
        public float ScaleFactor { get; set; } = NativeImage.DefaultScaleFactor;
        /// <summary>
        /// Utility conversion for obsolete class
        /// </summary>
        public static implicit operator ImageOptions(ToDataUrlOptions o) => new () {ScaleFactor = o.ScaleFactor};
    }
}
