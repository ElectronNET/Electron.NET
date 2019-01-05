using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public sealed class HostHook
    {
        private static HostHook _electronHostHook;
        private static object _syncRoot = new object();

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

        public void Call(string socketEventName, params dynamic[] arguments)
        {
            BridgeConnector.Socket.Emit(socketEventName, arguments);
        }

        public Task<T> CallAsync<T>(string socketEventName, params dynamic[] arguments)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On(socketEventName + "Complete" + guid, (result) =>
            {
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
