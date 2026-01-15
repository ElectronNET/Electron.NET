namespace ElectronNET.Runtime.Helpers
{
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    internal static class PortHelper
    {
        public static int GetFreePort(int? defaultPost)
        {
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(e => e.Port).ToList();
            var localAddresses = GetLocalAddresses();

            int port = defaultPost ?? 8000;

            while (true)
            {
                if (!listeners.Contains(port) && TryBindPort(port, localAddresses))
                {
                    return port;
                }

                port += 2;
            }
        }

        private static HashSet<IPAddress> GetLocalAddresses()
        {
            var addresses = new HashSet<IPAddress>
            {
                IPAddress.Any,
                IPAddress.IPv6Any,
                IPAddress.Loopback,
                IPAddress.IPv6Loopback
            };

            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus != OperationalStatus.Up)
                    {
                        continue;
                    }

                    var ipProperties = networkInterface.GetIPProperties();

                    foreach (var unicastAddress in ipProperties.UnicastAddresses)
                    {
                        addresses.Add(unicastAddress.Address);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return addresses;
        }

        private static bool TryBindPort(int port, HashSet<IPAddress> addresses)
        {
            TcpListener listener = null;

            foreach (var address in addresses)
            {
                try
                {
                    listener = new TcpListener(address, port);
                    listener.Start();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    listener?.Stop();
                }
            }

            return true;
        }
    }
}