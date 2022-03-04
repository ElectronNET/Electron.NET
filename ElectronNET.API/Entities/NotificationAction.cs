using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [SupportedOSPlatform("macos")]
    public class NotificationAction
    {
        /// <summary>
        /// The label for the given action.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The type of action, can be button.
        /// </summary>
        public string Type { get; set; }
    }
}