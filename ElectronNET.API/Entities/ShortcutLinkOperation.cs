using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the ShortcutLinkOperation enumeration.
    /// </summary>
    public enum ShortcutLinkOperation
    {
        /// <summary>
        /// Creates a new shortcut, overwriting if necessary.
        /// </summary>
        [Description("create")]
        Create,

        /// <summary>
        /// Updates specified properties only on an existing shortcut.
        /// </summary>
        [Description("update")]
        Update,

        /// <summary>
        /// Overwrites an existing shortcut, fails if the shortcut doesn't exist.
        /// </summary>
        [Description("replace")]
        Replace
    }
}
