namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateFromBufferOptions
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
        public float ScaleFactor { get; set; } = 1.0f;
    }
}