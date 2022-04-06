namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for creating a new <see cref="NativeImage"/>
    /// </summary>
    public class CreateOptions
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
        
    }
}