using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Allows you to execute native JavaScript/TypeScript code from the host process.
    /// 
    /// It is only possible if the Electron.NET CLI has previously added an 
    /// ElectronHostHook directory:
    /// <c>electronize add HostHook</c>
    /// </summary>
    public sealed class HostHook
    {
        private static HostHook _electronHostHook;
        private static object _syncRoot = new object();
        private string oneCallguid = Guid.NewGuid().ToString();

        internal HostHook()
        {
        }

        internal static HostHook Instance
        {
            get
            {
                if (_electronHostHook == null)
                {
                    lock (_syncRoot)
                    {
                        if (_electronHostHook == null)
                        {
                            _electronHostHook = new HostHook();
                        }
                    }
                }

                return _electronHostHook;
            }
        }

        /// <summary>
        /// Execute native JavaScript/TypeScript code.
        /// </summary>
        /// <param name="socketEventName">Socket name registered on the host.</param>
        /// <param name="arguments">Optional parameters.</param>
        public void Call(string socketEventName, params dynamic[] arguments)
        {
            BridgeConnector.Socket.On<JsonElement>(socketEventName + "Error" + oneCallguid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + oneCallguid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result.GetString());
            });

            BridgeConnector.Socket.Emit(socketEventName, arguments, oneCallguid);
        }

        /// <summary>
        /// Execute native JavaScript/TypeScript code.
        /// </summary>
        /// <typeparam name="T">Results from the executed host code.</typeparam>
        /// <param name="socketEventName">Socket name registered on the host.</param>
        /// <param name="arguments">Optional parameters.</param>
        /// <returns></returns>
        public Task<T> CallAsync<T>(string socketEventName, params dynamic[] arguments)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On<JsonElement>(socketEventName + "Error" + guid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + guid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result.GetString());
                taskCompletionSource.SetException(new Exception($"Host Hook Exception {result}"));
            });

            BridgeConnector.Socket.On<JsonElement>(socketEventName + "Complete" + guid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + guid);
                BridgeConnector.Socket.Off(socketEventName + "Complete" + guid);
                T data = default;

                try
                {
                    data = result.Deserialize<T>(Serialization.ElectronJson.Options);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                    //throw new InvalidCastException("Return value does not match with the generic type.", exception);
                }

                taskCompletionSource.SetResult(data);
            });

            BridgeConnector.Socket.Emit(socketEventName, arguments, guid);

            return taskCompletionSource.Task;
        }


    }
}
