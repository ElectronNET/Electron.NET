using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("macos")]
    public class NotificationAction
    {
        /// <summary>
        /// Gets or sets the label for the action.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the type of action; can be 'button'.
        /// </summary>
        public string Type { get; set; }
    }
}