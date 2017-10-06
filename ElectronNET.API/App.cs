using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quobject.SocketIoClientDotNet.Client;
using System;

namespace ElectronNET.API
{
    public static class App
    {
        private static Socket _socket;
        private static JsonSerializer _jsonSerializer;

        public static IpcMain IpcMain { get; private set; }

        public static void OpenWindow(int width, int height, bool show)
        {
            _jsonSerializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _socket = IO.Socket("http://localhost:" + BridgeSettings.SocketPort);
            _socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Verbunden!");

                var browserWindowOptions = new BrowserWindowOptions() {
                    Height = height,
                    Width = width,
                    Show = show
                };

                _socket.Emit("createBrowserWindow", JObject.FromObject(browserWindowOptions, _jsonSerializer));
            });

            IpcMain = new IpcMain(_socket);
        }

        public static void CreateNotification(NotificationOptions notificationOptions)
        {
            _socket.Emit("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }
    }
}
