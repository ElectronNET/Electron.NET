namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// PEM encoded data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Fingerprint of the certificate
        /// </summary>
        public string Fingerprint { get; set; }

        /// <summary>
        /// Issuer principal
        /// </summary>
        public CertificatePrincipal Issuer { get; set; }

        /// <summary>
        /// Issuer certificate (if not self-signed)
        /// </summary>
        public Certificate IssuerCert { get; set; }

        /// <summary>
        /// Issuer's Common Name
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// Hex value represented string
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Subject principal
        /// </summary>
        public CertificatePrincipal Subject { get; set; }

        /// <summary>
        /// Subject's Common Name
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// End date of the certificate being valid in seconds
        /// </summary>
        public int ValidExpiry { get; set; }

        /// <summary>
        /// Start date of the certificate being valid in seconds
        /// </summary>
        public int ValidStart { get; set; }
    }
}