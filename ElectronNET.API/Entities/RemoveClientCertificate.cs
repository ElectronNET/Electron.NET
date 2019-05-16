namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoveClientCertificate
    {
        /// <summary>
        /// Origin of the server whose associated client certificate must be removed from
        /// the cache.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// clientCertificate.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin">Origin of the server whose associated client certificate 
        /// must be removed from the cache.</param>
        /// <param name="type">clientCertificate.</param>
        public RemoveClientCertificate(string origin, string type)
        {
            Origin = origin;
            Type = type;
        }
    }
}
