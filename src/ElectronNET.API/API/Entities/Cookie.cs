namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Cookie structure as used by Electron session.cookies APIs.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class Cookie
    {
        /// <summary>
        /// Gets or sets the name of the cookie.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the cookie.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the domain of the cookie; this will be normalized with a preceding dot so that it's also valid for subdomains.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is a host-only cookie; this will only be true if no domain was passed.
        /// </summary>
        public bool? HostOnly { get; set; }

        /// <summary>
        /// Gets or sets the path of the cookie.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is marked as secure.
        /// </summary>
        public bool? Secure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is marked as HTTP only.
        /// </summary>
        public bool? HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is a session cookie or a persistent cookie with an expiration date.
        /// </summary>
        public bool? Session { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the cookie as the number of seconds since the UNIX epoch. Not provided for session cookies.
        /// </summary>
        public double? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the SameSite policy applied to this cookie. Can be "unspecified", "no_restriction", "lax" or "strict".
        /// </summary>
        public string SameSite { get; set; }
    }
}