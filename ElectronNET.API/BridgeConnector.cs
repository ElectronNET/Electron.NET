namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        private static SocketIoFacade _socket;
        private static readonly object SyncRoot = new();

        public static SocketIoFacade Socket
        {
            get
            {
                if (_socket == null)
                {
                    lock (SyncRoot)
                    {
                        if (_socket == null)
                        {

                            string socketUrl = HybridSupport.IsElectronActive
                                ? $"http://localhost:{BridgeSettings.SocketPort}"
                                : "http://localhost";

                            _socket = new SocketIoFacade(socketUrl);
                            _socket.Connect();
                        }
                    }
                }

                return _socket;
            }
        }
    }
}