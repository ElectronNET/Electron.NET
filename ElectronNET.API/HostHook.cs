using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Allows you to execute native JavaScript/TypeScript code from the host process.
    /// 
    /// It is only possible if the Electron.NET CLI has previously added an 
    /// ElectronHostHook directory:
    /// <c>electronize add HostHook</c>
    /// </summary>
    public sealed class HostHook : IHostHook
    {
        private static HostHook _electronHostHook;
        private static readonly object _syncRoot = new();
        readonly string oneCallguid = Guid.NewGuid().ToString();

        internal HostHook() { }

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
            BridgeConnector.On<string>(socketEventName + "Error" + oneCallguid, (result) =>
            {
                BridgeConnector.Off(socketEventName + "Error" + oneCallguid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result);
            });

            BridgeConnector.Emit(socketEventName, arguments, oneCallguid);
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
            var taskCompletionSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On<string>(socketEventName + "Error" + guid, (result) =>
            {
                BridgeConnector.Off(socketEventName + "Error" + guid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result);
                taskCompletionSource.SetException(new Exception($"Host Hook Exception {result}"));
            });

            BridgeConnector.On<T>(socketEventName + "Complete" + guid, (result) =>
            {
                BridgeConnector.Off(socketEventName + "Error" + guid);
                BridgeConnector.Off(socketEventName + "Complete" + guid);
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit(socketEventName, arguments, guid);

            return taskCompletionSource.Task;
        }
    }
}
