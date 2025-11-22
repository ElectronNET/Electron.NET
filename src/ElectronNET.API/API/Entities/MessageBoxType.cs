namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Message box type for dialog.showMessageBox/showMessageBoxSync.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public enum MessageBoxType
    {
        /// <summary>
        /// The none
        /// </summary>
        none,

        /// <summary>
        /// The information
        /// </summary>
        info,

        /// <summary>
        /// The error
        /// </summary>
        error,

        /// <summary>
        /// The question
        /// </summary>
        question,

        /// <summary>
        /// The warning
        /// </summary>
        warning
    }
}