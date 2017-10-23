using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Display
    {
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Can be available, unavailable, unknown.
        /// </summary>
        public string TouchSupport { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle WorkArea { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Size WorkAreaSize { get; set; }
    }
}