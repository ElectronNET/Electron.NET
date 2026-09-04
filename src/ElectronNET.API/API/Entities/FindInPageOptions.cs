namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for <see cref="WebContents.FindInPageAsync"/>.
    /// </summary>
    public class FindInPageOptions
    {
        /// <summary>
        /// Whether to search forward or backward, defaults to true.
        /// </summary>
        public bool? Forward { get; set; }

        /// <summary>
        /// Whether to begin a new text finding session with this request. Should be true
        /// for initial requests, and false for subsequent requests. Defaults to false.
        /// </summary>
        public bool? FindNext { get; set; }

        /// <summary>
        /// Whether search should be case-sensitive, defaults to false.
        /// </summary>
        public bool? MatchCase { get; set; }
    }
}
