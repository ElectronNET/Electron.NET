namespace ElectronNET.Runtime.Helpers
{
    using System.Linq;
    using System.Net.NetworkInformation;

    internal static class PortHelper
    {
        public static int GetFreePort(int? defaultPost)
        {
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(e => e.Port).ToList();

            int port = defaultPost ?? 8000;

            while (true)
            {
                if (!listeners.Contains(port))
                {
                    return port;
                }

                port += 2;
            }
        }
    }
}