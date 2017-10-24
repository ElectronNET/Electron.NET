namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Opens the devtools with specified dock state, can be right, bottom, undocked,
    /// detach.Defaults to last used dock state.In undocked mode it's possible to dock
    /// back.In detach mode it's not.
    /// </summary>
    public enum DevToolsMode
    {
        /// <summary>
        /// The right
        /// </summary>
        right,

        /// <summary>
        /// The bottom
        /// </summary>
        bottom,

        /// <summary>
        /// The undocked
        /// </summary>
        undocked,

        /// <summary>
        /// The detach
        /// </summary>
        detach
    }
}