namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class CertificateTrustDialogOptions
    {
        /// <summary>
        /// The certificate to trust/import.
        /// </summary>
        public Certificate Certificate { get; set; }

        /// <summary>
        /// The message to display to the user.
        /// </summary>
        public string Message { get; set; }
    }
}