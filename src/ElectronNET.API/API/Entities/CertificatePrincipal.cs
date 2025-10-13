namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class CertificatePrincipal
    {
        /// <summary>
        /// Common Name
        /// </summary>
        public string CommonName { get; set; }

        /// <summary>
        /// Country or region
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Locality
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Organization names
        /// </summary>
        public string[] Organizations { get; set; }

        /// <summary>
        /// Organization Unit names
        /// </summary>
        public string[] OrganizationUnits { get; set; }

        /// <summary>
        /// State or province
        /// </summary>
        public string State { get; set; }
    }
}