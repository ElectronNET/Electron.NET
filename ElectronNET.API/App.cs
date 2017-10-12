using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public static class App
    {
        public static IpcMain IpcMain { get; private set; }

        private static Socket _socket;
        private static JsonSerializer _jsonSerializer;

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

                var browserWindowOptions = new BrowserWindowOptions()
                {
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

        public static void Quit()
        {
            _socket.Emit("appQuit");
        }

        public static void Exit(int exitCode = 0)
        {
            _socket.Emit("appExit", exitCode);
        }

        public static void Relaunch()
        {
            _socket.Emit("appRelaunch");
        }

        public static void Relaunch(RelaunchOptions relaunchOptions)
        {
            _socket.Emit("appRelaunch", JObject.FromObject(relaunchOptions, _jsonSerializer));
        }

        public static void Focus()
        {
            _socket.Emit("appFocus");
        }

        public static void Hide()
        {
            _socket.Emit("appHide");
        }

        public static void Show()
        {
            _socket.Emit("appShow");
        }

        public async static Task<string> GetAppPathAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetAppPathCompleted", (path) =>
            {
                taskCompletionSource.SetResult(path.ToString());
            });

            _socket.Emit("appGetAppPath");

            return await taskCompletionSource.Task;
        }

        public async static Task<string> GetPathAsync(PathName pathName)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetPathCompleted", (path) =>
                    {
                        taskCompletionSource.SetResult(path.ToString());
                    });

            _socket.Emit("appGetPath", pathName.ToString());

            return await taskCompletionSource.Task;
        }

        public static void Blub2() { }
    }
}
