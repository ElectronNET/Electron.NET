using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities {
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