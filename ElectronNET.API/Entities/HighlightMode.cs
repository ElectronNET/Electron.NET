namespace ElectronNET.API.Entities
{
    public enum HighlightMode
    {
        /// <summary>
        /// Highlight the tray icon when it is clicked and also when its context menu is open. This is the default.
        /// </summary>
        selection,

        /// <summary>
        /// Always highlight the tray icon.
        /// </summary>
        always,

        /// <summary>
        /// Never highlight the tray icon.
        /// </summary>
        never
    }
}