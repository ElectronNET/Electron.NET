namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Display
    {
        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Unique identifier associated with the display.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Can be 0, 90, 180, 270, represents screen rotation in clock-wise degrees.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Output device's pixel scale factor.
        /// </summary>
        public int ScaleFactor { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size Size { get; set; }

        /// <summary>
        /// Can be available, unavailable, unknown.
        /// </summary>
        public string TouchSupport { get; set; }

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