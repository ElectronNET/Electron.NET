namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CookieFilter
    {
        /// <summary>
        /// (optional) - Retrieves cookies which are associated with url. Empty implies retrieving cookies of all URLs.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// (optional) - Filters cookies by name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (optional) - Retrieves cookies whose domains match or are subdomains of domains.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// (optional) - Retrieves cookies whose path matches path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// (optional) - Filters cookies by their Secure property.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// (optional) - Filters out session or persistent cookies.
        /// </summary>
        public bool Session { get; set; }

        /// <summary>
        /// (optional) - Filters cookies by httpOnly.
        /// </summary>
        public bool HttpOnly { get; set; }
    }
}