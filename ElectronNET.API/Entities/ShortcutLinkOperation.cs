namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public enum ShortcutLinkOperation
    {
        /// <summary>
        /// Creates a new shortcut, overwriting if necessary.
        /// </summary>
        create,

        /// <summary>
        /// Updates specified properties only on an existing shortcut.
        /// </summary>
        update,

        /// <summary>
        /// Overwrites an existing shortcut, fails if the shortcut doesn’t exist.
        /// </summary>
        replace
    }
}