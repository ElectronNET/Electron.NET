namespace ElectronNET.API.Entities
{
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