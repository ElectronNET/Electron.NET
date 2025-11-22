using System.ComponentModel;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the DockBounceType enumeration.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("macOS")]
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