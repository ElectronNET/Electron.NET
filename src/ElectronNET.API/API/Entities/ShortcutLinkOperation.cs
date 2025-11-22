using System.ComponentModel;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the ShortcutLinkOperation enumeration.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("Windows")]
    public enum ShortcutLinkOperation
    {
        /// <summary>
        /// Creates a new shortcut, overwriting if necessary.
        /// </summary>
        Create,

        /// <summary>
        /// Updates specified properties only on an existing shortcut.
        /// </summary>
        Update,

        /// <summary>
        /// Overwrites an existing shortcut, fails if the shortcut doesn't exist.
        /// </summary>
        Replace
    }
}