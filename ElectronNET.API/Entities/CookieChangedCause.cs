using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities {

    public class CookieRemovedResponse
    {
        public Cookie cookie {get;set;}

        public CookieChangedCause cause { get; set; }
        public bool removed { get; set; }
    }

    /// <summary>
    /// The cause of the change 
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CookieChangedCause 
    {
        /// <summary>
        ///The cookie was changed directly by a consumer's action.
        /// </summary>
        [JsonProperty("explicit")]
        @explicit,

        /// <summary>
        /// The cookie was automatically removed due to an insert operation that overwrote it.
        /// </summary>
        overwrite,

        /// <summary>
        ///  The cookie was automatically removed as it expired.
        /// </summary>
        expired,

        /// <summary>
        ///  The cookie was automatically evicted during garbage collection.
        /// </summary>
        evicted,

        /// <summary>
        ///   The cookie was overwritten with an already-expired expiration date.
        /// </summary>
        [JsonProperty("expired_overwrite")]
        expiredOverwrite
    }
}