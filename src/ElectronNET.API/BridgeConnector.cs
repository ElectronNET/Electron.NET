using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

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

                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        taskCompletionSource.SetResult( ((JObject)value).ToObject<T>() );
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
                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        taskCompletionSource.SetResult( ((JArray)value).ToObject<T>() );
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
        
    }
}
