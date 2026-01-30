namespace ElectronNET.AspNet.Services
{
    /// <summary>
    /// Service for validating authentication tokens from Electron clients.
    /// Used to ensure only the Electron process spawned by this .NET instance can connect.
    /// </summary>
    public interface IElectronAuthenticationService
    {
        /// <summary>
        /// Sets the expected authentication token for this instance.
        /// Should be called when launching Electron with the generated token.
        /// </summary>
        /// <param name="token">The authentication token</param>
        void SetExpectedToken(string token);

        /// <summary>
        /// Validates an incoming token against the expected token.
        /// Uses constant-time comparison to prevent timing attacks.
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>True if token is valid, false otherwise</returns>
        bool ValidateToken(string token);
    }
}
