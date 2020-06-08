using System.ComponentModel;

namespace ElectronNET.API.Entities {
    /// <summary>
    /// 
    /// </summary>
    public class CookieDetails {
        /// <summary>
        /// The URL to associate the cookie with. The callback will be rejected if the URL is invalid.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// (optional) - The name of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Name { get; set; }

        /// <summary>
        /// (optional) - The value of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")] 
        public string Value { get; set; }

        /// <summary>
        /// (optional) - The domain of the cookie; this will be normalized with a preceding dot so that it's also valid for subdomains. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Domain { get; set; }

        /// <summary>
        /// (optional) - The path of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")] 
        public string Path { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is marked as secure. Defaults to false.
        /// </summary>
        [DefaultValue(false)] 
        public bool Secure { get; set; }

        /// <summary>
        /// (optional) - Whether the cookie is marked as HTTP only. Defaults to false.
        /// </summary>
        [DefaultValue(false)]
        public bool HttpOnly { get; set; }

        /// <summary>
        /// (optional) - The expiration date of the cookie as the number of seconds since the UNIX epoch. 
        /// If omitted then the cookie becomes a session cookie and will not be retained between sessions.
        /// </summary>
        [DefaultValue(0)]
        public long ExpirationDate { get; set; }
    }
}
