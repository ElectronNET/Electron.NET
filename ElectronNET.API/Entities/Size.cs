namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Size
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }
        
        /// <summary>
        /// Utility implicit conversion
        /// </summary>
        public static implicit operator SixLabors.ImageSharp.Size(Size s) =>
            new (s.Width, s.Height);

        /// <summary>
        /// Utility implicit conversion
        /// </summary>
        public static implicit operator Size(SixLabors.ImageSharp.Size s) =>
            new (){Height = s.Height, Width = s.Width};
    }
}