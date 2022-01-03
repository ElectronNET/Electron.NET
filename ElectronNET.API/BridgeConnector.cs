using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        private static Socket _socket;
        private static object _syncRoot = new object();

        public static Socket Socket
        {
            get
            {
                if(_socket == null && HybridSupport.IsElectronActive)
                {
                    lock (_syncRoot)
                    {
                        if (_socket == null && HybridSupport.IsElectronActive)
                        {
                            _socket = IO.Socket("http://localhost:" + BridgeSettings.SocketPort);
                            _socket.On(Socket.EVENT_CONNECT, () =>
                            {
                                Console.WriteLine("BridgeConnector connected!");
                            });
                        }
                    }
                }
                else if(_socket == null && !HybridSupport.IsElectronActive)
                {
                    lock (_syncRoot)
                    {
                        if (_socket == null && !HybridSupport.IsElectronActive)
                        {
                            _socket = IO.Socket(new Uri("http://localhost"), new IO.Options { AutoConnect = false });
                        }
                    }
                }

                return _socket;
            }
        }

        public static async Task<T> GetValueOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);

                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        taskCompletionSource.SetResult( new JValue(value).ToObject<T>() );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') exception: {e.Message}. Socket loop hung.");
                    }
                });

                BridgeConnector.Socket.Emit(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> GetObjectOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);
                    taskCompletionSource.SetResult( ((JObject)value).ToObject<T>() );
                });

                BridgeConnector.Socket.Emit(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> GetArrayOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);
                    taskCompletionSource.SetResult( ((JArray)value).ToObject<T>() );
                });

                BridgeConnector.Socket.Emit(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }
        
    }
}
