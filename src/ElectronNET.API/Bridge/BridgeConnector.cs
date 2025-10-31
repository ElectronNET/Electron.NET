#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        public static SocketIoFacade Socket
        {
            get
            {
                return ElectronNetRuntime.GetSocket();
            }
        }
    }
}