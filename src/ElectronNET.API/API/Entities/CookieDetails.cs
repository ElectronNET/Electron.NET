using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CookieDetails
    {
        /// <summary>
        /// Gets or sets the URL to associate the cookie with. The operation will be rejected if the URL is invalid.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the domain of the cookie; this will be normalized with a preceding dot so that it's also valid for subdomains. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the cookie. Empty by default if omitted.
        /// </summary>
        [DefaultValue("")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie should be marked as secure. Defaults to false unless the SameSite policy is set to <c>no_restriction</c> (SameSite=None).
        /// </summary>
        [DefaultValue(false)]
        public bool Secure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie should be marked as HTTP only. Defaults to false.
        /// </summary>
        [DefaultValue(false)]
        public bool HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the cookie as the number of seconds since the UNIX epoch. If omitted, the cookie becomes a session cookie and will not be retained between sessions.
        /// </summary>
        [DefaultValue(0)]
        public double ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the SameSite policy to apply to this cookie. Can be "unspecified", "no_restriction", "lax" or "strict". Default is "lax".
        /// </summary>
        public string SameSite { get; set; }
    }
}