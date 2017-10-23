namespace ElectronNET.API.Entities
{
    public class Data
    {
        public string Text { get; set; }
        public string Html { get; set; }
        public string Rtf { get; set; }

        /// <summary>
        /// The title of the url at text.
        /// </summary>
        public string Bookmark { get; set; }
    }
}