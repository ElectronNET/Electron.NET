namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for <see cref="WebContents.AdjustSelection"/>.
    /// </summary>
    public class AdjustSelectionOptions
    {
        /// <summary>
        /// Amount to shift the start index of the current selection.
        /// </summary>
        public int? Start { get; set; }

        /// <summary>
        /// Amount to shift the end index of the current selection.
        /// </summary>
        public int? End { get; set; }
    }
}
