namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Display
    {
        /// <summary>
        /// Can be available, unavailable, unknown.
        /// </summary>
        public string AccelerometerSupport { get; set; }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds of the display in DIP points.
        /// </value>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// The number of bits per pixel.
        /// </summary>
        public int ColorDepth { get; set; }

        /// <summary>
        /// Represent a color space (three-dimensional object which contains all realizable color combinations) for the purpose of color conversions.
        /// </summary>
        public string ColorSpace { get; set; }

        /// <summary>
        /// The number of bits per color component.
        /// </summary>
        public int DepthPerComponent { get; set; }

        /// <summary>
        /// The display refresh rate.
        /// </summary>
        public int DisplayFrequency { get; set; }

        /// <summary>
        /// Unique identifier associated with the display.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// true for an internal display and false for an external display.
        /// </summary>
        public bool Internal { get; set; }

        /// <summary>
        /// User-friendly label, determined by the platform.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Can be 0, 90, 180, 270, represents screen rotation in clock-wise degrees.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Output device's pixel scale factor.
        /// </summary>
        public double ScaleFactor { get; set; }

        /// <summary>
        /// Can be available, unavailable, unknown.
        /// </summary>
        public string TouchSupport { get; set; }

        /// <summary>
        /// Whether or not the display is a monochrome display.
        /// </summary>
        public bool Monochrome { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets the work area.
        /// </summary>
        /// <value>
        /// The work area.
        /// </value>
        public Rectangle WorkArea { get; set; }

        /// <summary>
        /// Gets or sets the size of the work area.
        /// </summary>
        /// <value>
        /// The size of the work area.
        /// </value>
        public Size WorkAreaSize { get; set; }
    }
}