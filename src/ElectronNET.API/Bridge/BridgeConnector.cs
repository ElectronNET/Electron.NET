#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API
{
    using ElectronNET.Runtime;

    internal static class BridgeConnector
    {
        public static SocketIoFacade Socket
        {
            get
            {
                return ElectronHostEnvironment.Current.GetSocket();
            }
        }
    }
}