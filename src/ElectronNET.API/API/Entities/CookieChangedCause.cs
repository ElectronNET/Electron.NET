namespace ElectronNET.API.Entities
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The cause of the cookie change (per Electron Cookies 'changed' event).
    /// </summary>
    public enum CookieChangedCause
    {
        /// <summary>
        /// The cookie was changed directly by a consumer's action.
        /// </summary>
        @explicit,

        /// <summary>
        /// The cookie was automatically removed due to an insert operation that overwrote it.
        /// </summary>
        overwrite,

        /// <summary>
        /// The cookie was automatically removed as it expired.
        /// </summary>
        expired,

        /// <summary>
        /// The cookie was automatically evicted during garbage collection.
        /// </summary>
        evicted,

        /// <summary>
        /// The cookie was overwritten with an already-expired expiration date.
        /// </summary>
        [JsonPropertyName("expired_overwrite")]
        expiredOverwrite
    }
}