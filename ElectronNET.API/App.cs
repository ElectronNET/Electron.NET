using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quobject.SocketIoClientDotNet.Client;
using System;

namespace ElectronNET.API
{
    public class App
    {
        private readonly Socket _socket;
        private readonly JsonSerializer _jsonSerializer;

        public App(int width, int height, bool show)
        {
            _jsonSerializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var socket = IO.Socket("http://localhost:3000");
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Verbunden!");

                var browserWindowOptions = new BrowserWindowOptions() {
                    Height = height,
                    Width = width,
                    Show = show
                };

                socket.Emit("createBrowserWindow", JObject.FromObject(browserWindowOptions, _jsonSerializer));
                socket.Emit("createNotification");
            });
        }
    }
}
