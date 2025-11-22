using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for app.importCertificate(options) on Linux.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("linux")]
    public class ImportCertificateOptions
    {
        /// <summary>
        /// Path for the pkcs12 file.
        /// </summary>
        public string Certificate { get; set; }

        /// <summary>
        /// Passphrase for the certificate.
        /// </summary>
        public string Password { get; set; }
    }
}