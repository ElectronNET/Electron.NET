using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class RemovePassword
    {
        /// <summary>
        /// When provided, the authentication info related to the origin will only be
        /// removed otherwise the entire cache will be cleared.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Credentials of the authentication. Must be provided if removing by origin.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Realm of the authentication. Must be provided if removing by origin.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Scheme of the authentication. Can be basic, digest, ntlm, negotiate. 
        /// Must be provided if removing by origin.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Scheme Scheme { get; set; }

        /// <summary>
        /// password.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Credentials of the authentication. Must be provided if removing by origin.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">password.</param>
        public RemovePassword(string type)
        {
            Type = type;
        }
    }
}
