namespace ElectronNET.API.Entities {
    /// <summary>
    /// 
    /// </summary>
    public class Cookie {
        /// <summary>
        /// The name of the cookie.
        /// </summary>
        public string Name { get; set;}

        /// <summary>
        /// The value of the cookie.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// (optional) - The domain of the cookie; this will be normalized with a preceding dot so that it's also valid for subdomains.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is a host-only cookie; this will only be true if no domain was passed.
        /// </summary>
        public bool HostOnly { get; set; }

        /// <summary>
        /// (optional) - The path of the cookie.
        /// </summary>
        public string Path  { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is marked as secure.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is marked as HTTP only.
        /// </summary>
        public bool HttpOnly  { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is a session cookie or a persistent cookie with an expiration date.
        /// </summary>
        public bool Session { get; set; }

        /// <summary>
        /// (optional) - The expiration date of the cookie as the number of seconds since the UNIX epoch. Not provided for session cookies.
        /// </summary>
        public long ExpirationDate { get; set; }
    }
}
