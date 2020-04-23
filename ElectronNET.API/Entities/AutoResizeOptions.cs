using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoResizeOptions
    {
        /// <summary>
        /// If `true`, the view's width will grow and shrink together with the window.
        /// `false` by default.
        /// </summary>
        [DefaultValue(false)]
        public bool Width { get; set; } = false;

        /// <summary>
        /// If `true`, the view's height will grow and shrink together with the window.
        /// `false` by default.
        /// </summary>
        [DefaultValue(false)]
        public bool Height { get; set; } = false;

        /// <summary>
        /// If `true`, the view's x position and width will grow and shrink proportionally
        /// with the window. `false` by default.
        /// </summary>
        [DefaultValue(false)]
        public bool Horizontal { get; set; } = false;

        /// <summary>
        /// If `true`, the view's y position and height will grow and shrink proportionally
        /// with the window. `false` by default.
        /// </summary>
        [DefaultValue(false)]
        public bool Vertical { get; set; } = false;
    }
}
