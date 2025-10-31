using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the DockBounceType enumeration.
    /// </summary>
    public enum DockBounceType
    {
        /// <summary>
        /// Dock icon will bounce until either the application becomes active or the request is canceled.
        /// </summary>
        [Description("critical")]
        Critical,

        /// <summary>
        /// The dock icon will bounce for one second.
        /// </summary>
        [Description("informational")]
        Informational
    }
}