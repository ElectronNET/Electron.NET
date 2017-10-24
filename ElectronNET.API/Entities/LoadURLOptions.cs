namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadURLOptions
    {
        /// <summary>
        /// A HTTP Referrer url.
        /// </summary>
        public string HttpReferrer { get; set; }

        /// <summary>
        /// A user agent originating the request.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Base url (with trailing path separator) for files to be loaded by the data url.
        /// This is needed only if the specified url is a data url and needs to load other
        /// files.
        /// </summary>
        public string BaseURLForDataURL { get; set; }
    }
}