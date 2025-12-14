namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Result returned by dialog.showMessageBox / dialog.showMessageBoxSync.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class MessageBoxResult
    {
        /// <summary>
        /// The index of the clicked button.
        /// </summary>
        public int Response { get; set; }

        /// <summary>
        /// The checked state of the checkbox if CheckboxLabel was set; otherwise false.
        /// </summary>
        public bool CheckboxChecked { get; set; }
    }
}