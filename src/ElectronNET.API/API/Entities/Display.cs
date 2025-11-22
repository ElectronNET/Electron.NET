namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class Display
    {
        /// <summary>
        /// Gets or sets the accelerometer support status; can be 'available', 'unavailable', or 'unknown'.
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
        /// Gets or sets the number of bits per pixel.
        /// </summary>
        public int ColorDepth { get; set; }

        /// <summary>
        /// Gets or sets the color space description used for color conversions.
        /// </summary>
        public string ColorSpace { get; set; }

        /// <summary>
        /// Gets or sets the number of bits per color component.
        /// </summary>
        public int DepthPerComponent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the display is detected by the system.
        /// </summary>
        public bool Detected { get; set; }

        /// <summary>
        /// Gets or sets the display refresh rate.
        /// </summary>
        public double DisplayFrequency { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier associated with the display. A value of -1 means the display is invalid or the correct id is not yet known, and a value of -10 means the display is a virtual display assigned to a unified desktop.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the display is internal (true) or external (false).
        /// </summary>
        public bool Internal { get; set; }

        /// <summary>
        /// Gets or sets the user-friendly label, determined by the platform.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the maximum cursor size in native pixels.
        /// </summary>
        public Size MaximumCursorSize { get; set; }

        /// <summary>
        /// Gets or sets the display's origin in pixel coordinates. Only available on windowing systems that position displays in pixel coordinates (e.g., X11).
        /// </summary>
        public Point NativeOrigin { get; set; }

        /// <summary>
        /// Gets or sets the screen rotation in clock-wise degrees. Can be 0, 90, 180, or 270.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Gets or sets the output device's pixel scale factor.
        /// </summary>
        public double ScaleFactor { get; set; }

        /// <summary>
        /// Gets or sets the touch support status; can be 'available', 'unavailable', or 'unknown'.
        /// </summary>
        public string TouchSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the display is monochrome.
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
        /// Gets or sets the work area of the display in DIP points.
        /// </summary>
        /// <value>
        /// The work area of the display in DIP points.
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