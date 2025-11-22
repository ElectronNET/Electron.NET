using System.Runtime.Versioning;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public enum DisplayBalloonIconType
    {
        none,
        info,
        warning,
        error,
        custom
    }

    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("Windows")]
    public class DisplayBalloonOptions
    {
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// (optional) - Icon type for the balloon: none, info, warning, error or custom.
        /// Default is custom.
        /// </summary>
        public DisplayBalloonIconType IconType { get; set; } = DisplayBalloonIconType.custom;

        /// <summary>
        /// (optional) - Use the large version of the icon. Default is true.
        /// Maps to Windows NIIF_LARGE_ICON.
        /// </summary>
        public bool LargeIcon { get; set; } = true;

        /// <summary>
        /// (optional) - Do not play the associated sound. Default is false.
        /// Maps to Windows NIIF_NOSOUND.
        /// </summary>
        public bool NoSound { get; set; }

        /// <summary>
        /// (optional) - Do not display the balloon if the current user is in "quiet time".
        /// Default is false. Maps to Windows NIIF_RESPECT_QUIET_TIME.
        /// </summary>
        public bool RespectQuietTime { get; set; }
    }
}