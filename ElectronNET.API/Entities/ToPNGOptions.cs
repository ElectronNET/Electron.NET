using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use ImageOptions instead.")]
    public class ToPNGOptions
    {
        /// <summary>
        /// Gets or sets the scalefactor
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;
        /// <summary>
        /// Utility conversion for obsolete class
        /// </summary>
        public static implicit operator ImageOptions(ToPNGOptions o) => new () {ScaleFactor = o.ScaleFactor};
    }
}
