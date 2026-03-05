namespace ElectronNET.AspNet.Services
{
    /// <summary>
    /// Implementation of authentication service for Electron clients.
    /// Stores and validates the authentication token to ensure only the spawned Electron process can connect.
    /// </summary>
    public class ElectronAuthenticationService : IElectronAuthenticationService
    {
        private string _expectedToken;
        private readonly object _lock = new object();

        /// <inheritdoc />
        public void SetExpectedToken(string token)
        {
            lock (_lock)
            {
                _expectedToken = token;
            }
        }

        /// <inheritdoc />
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            lock (_lock)
            {
                if (string.IsNullOrEmpty(_expectedToken))
                    return false;

                // Constant-time comparison to prevent timing attacks
                return ConstantTimeEquals(token, _expectedToken);
            }
        }

        /// <summary>
        /// Performs constant-time string comparison to prevent timing attacks.
        /// </summary>
        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}