namespace ElectronNET.API.Entities
{
    /// <summary>
    /// The result of a text finding session, reported by the found-in-page event.
    /// </summary>
    public class FoundInPageResult
    {
        /// <summary>
        /// The identifier of the request returned by FindInPageAsync.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Position of the active match.
        /// </summary>
        public int ActiveMatchOrdinal { get; set; }

        /// <summary>
        /// Number of matches.
        /// </summary>
        public int Matches { get; set; }

        /// <summary>
        /// Coordinates of the first match region.
        /// </summary>
        public Rectangle SelectionArea { get; set; }

        /// <summary>
        /// Indicates whether more responses are to follow.
        /// </summary>
        public bool FinalUpdate { get; set; }
    }
}
