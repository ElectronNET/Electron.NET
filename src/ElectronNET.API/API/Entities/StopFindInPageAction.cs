namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Determines what happens to the selection when a text finding session is stopped.
    /// </summary>
    public enum StopFindInPageAction
    {
        /// <summary>
        /// Clear the selection.
        /// </summary>
        ClearSelection,

        /// <summary>
        /// Translate the selection into a normal selection.
        /// </summary>
        KeepSelection,

        /// <summary>
        /// Focus and click the selection node.
        /// </summary>
        ActivateSelection
    }
}
