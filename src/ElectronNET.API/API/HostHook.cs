using ElectronNET.API.Serialization;
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
            BridgeConnector.Socket.Once<string>(socketEventName + "Error" + oneCallguid, (result) =>
            {
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result);
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
            var tcs = new TaskCompletionSource<T>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string>(socketEventName + "Error" + guid, (result) =>
            {
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result);
                tcs.SetException(new Exception($"Host Hook Exception {result}"));
            });

            BridgeConnector.Socket.Once<JsonElement>(socketEventName + "Complete" + guid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + guid);
                T data = default;

                try
                {
                    data = result.Deserialize<T>(ElectronJson.Options);
                }
                catch (Exception exception)
                {
                    tcs.SetException(exception);
                }

                tcs.SetResult(data);
            });

            BridgeConnector.Socket.Emit(socketEventName, arguments, guid);

            return tcs.Task;
        }


    }
}
