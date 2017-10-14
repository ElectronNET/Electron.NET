using Quobject.SocketIoClientDotNet.Client;
using System;

namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        public static Socket Socket;

        public static void StartConnection()
        {
            Socket = IO.Socket("http://localhost:" + BridgeSettings.SocketPort);
            Socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("BridgeConnector connected!");
            });
        }
    }
}
