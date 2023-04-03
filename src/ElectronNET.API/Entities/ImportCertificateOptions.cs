namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ImportCertificateOptions
    {
        /// <summary>
        /// Path for the pkcs12 file.
        /// </summary>
        public string Certificate { get; set; }
        
        /// <summary>
        /// Passphrase for the certificate.
        /// </summary>
        public string Password {get; set; }
    }
}
