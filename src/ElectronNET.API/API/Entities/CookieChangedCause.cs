
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// The cause of the change 
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CookieChangedCause
    {
        /// <summary>
        ///The cookie was changed directly by a consumer's action.
        /// </summary>
        [JsonPropertyName("explicit")]
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
        [JsonPropertyName("expired_overwrite")]
        expiredOverwrite
    }
}
