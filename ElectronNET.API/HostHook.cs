using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
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
        string oneCallguid = Guid.NewGuid().ToString();

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
            BridgeConnector.Socket.On(socketEventName + "Error" + oneCallguid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + oneCallguid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result.ToString());
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

            BridgeConnector.Socket.On(socketEventName + "Error" + guid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + guid);
                Electron.Dialog.ShowErrorBox("Host Hook Exception", result.ToString());
            });

            BridgeConnector.Socket.On(socketEventName + "Complete" + guid, (result) =>
            {
                BridgeConnector.Socket.Off(socketEventName + "Error" + guid);
                BridgeConnector.Socket.Off(socketEventName + "Complete" + guid);
                T data;

                try
                {
                    if (result.GetType().IsValueType || result is string)
                    {
                        data = (T)result;
                    }
                    else
                    {
                        var token = JToken.Parse(result.ToString());
                        if (token is JArray)
                        {
                            data = token.ToObject<T>();
                        }
                        else if (token is JObject)
                        {
                            data = token.ToObject<T>();
                        }
                        else
                        {
                            data = (T)result;
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new InvalidCastException("Return value does not match with the generic type.", exception);
                }

                taskCompletionSource.SetResult(data);
            });

            BridgeConnector.Socket.Emit(socketEventName, arguments, guid);

            return taskCompletionSource.Task;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
