namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y { get; set; }

        /// <summary>
        /// Convert this <see cref="Point"/> to <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        public static implicit operator System.Drawing.Point(Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }
    }
}