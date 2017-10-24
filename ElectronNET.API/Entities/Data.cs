namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>
        /// The HTML.
        /// </value>
        public string Html { get; set; }


        /// <summary>
        /// Gets or sets the RTF.
        /// </summary>
        /// <value>
        /// The RTF.
        /// </value>
        public string Rtf { get; set; }

        /// <summary>
        /// The title of the url at text.
        /// </summary>
        public string Bookmark { get; set; }
    }
}