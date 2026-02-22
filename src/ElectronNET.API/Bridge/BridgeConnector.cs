#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API
{
    using ElectronNET.API.Bridge;

    internal static class BridgeConnector
    {
        public static ISocketConnection Socket
        {
            get
            {
                return ElectronNetRuntime.GetSocket();
            }
        }
    }
}