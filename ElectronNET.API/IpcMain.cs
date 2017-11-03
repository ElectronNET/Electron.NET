using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ElectronNET.API
{
    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public sealed class IpcMain
    {
        private static IpcMain _ipcMain;
        private static object _syncRoot = new Object();

        internal IpcMain() { }

        internal static IpcMain Instance
        {
            get
            {
                if(_ipcMain == null)
                {
                    lock (_syncRoot)
                    {
                        if(_ipcMain == null)
                        {
                            _ipcMain = new IpcMain();
                        }
                    }
                }

                return _ipcMain;
            }
        }

        /// <summary>
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void On(string channel, Action<object> listener)
        {
            BridgeConnector.Socket.Emit("registerIpcMainChannel", channel);
            BridgeConnector.Socket.On(channel, (args) => 
            {
                List<object> objectArray = FormatArguments(args);

                if(objectArray.Count == 1)
                {
                    listener(objectArray.First());
                }
                else
                {
                    listener(objectArray);
                }
            });
        }

        private List<object> FormatArguments(object args)
        {
            List<object> objectArray = ((JArray)args).ToObject<object[]>().ToList();

            for (int index = 0; index < objectArray.Count; index++)
            {
                var item = objectArray[index];
                if (item == null)
                {
                    objectArray.Remove(item);
                }
            }

            return objectArray;
        }

        /// <summary>
        /// Send a message to the renderer process synchronously via channel, 
        /// you can also send arbitrary arguments.
        /// 
        /// Note: Sending a synchronous message will block the whole renderer process,
        /// unless you know what you are doing you should never use it.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="listener"></param>
        public void OnSync(string channel, Func<object, object> listener)
        {
            BridgeConnector.Socket.Emit("registerSyncIpcMainChannel", channel);
            BridgeConnector.Socket.On(channel, (args) => {
                List<object> objectArray = FormatArguments(args);
                object parameter;
                if (objectArray.Count == 1)
                {
                    parameter = objectArray.First();
                }
                else
                {
                    parameter = objectArray;
                }

                var result = listener(parameter);
                BridgeConnector.Socket.Emit(channel + "Sync", result);
            });
        }

        /// <summary>
        /// Adds a one time listener method for the event. This listener is invoked only
        ///  the next time a message is sent to channel, after which it is removed.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void Once(string channel, Action<object> listener)
        {
            BridgeConnector.Socket.Emit("registerOnceIpcMainChannel", channel);
            BridgeConnector.Socket.On(channel, (args) =>
            {
                List<object> objectArray = FormatArguments(args);

                if (objectArray.Count == 1)
                {
                    listener(objectArray.First());
                }
                else
                {
                    listener(objectArray);
                }
            });
        }

        /// <summary>
        /// Removes listeners of the specified channel.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        public void RemoveAllListeners(string channel)
        {
            BridgeConnector.Socket.Emit("removeAllListenersIpcMainChannel", channel);
        }

        /// <summary>
        /// Send a message to the renderer process asynchronously via channel, you can also send
        /// arbitrary arguments. Arguments will be serialized in JSON internally and hence
        /// no functions or prototype chain will be included. The renderer process handles it by
        /// listening for channel with ipcRenderer module.
        /// </summary>
        /// <param name="browserWindow">BrowserWindow with channel.</param>
        /// <param name="channel">Channelname.</param>
        /// <param name="data">Arguments data.</param>
        public void Send(BrowserWindow browserWindow, string channel, params object[] data)
        {
            List<JObject> jobjects = new List<JObject>();
            List<JArray> jarrays = new List<JArray>();
            List<Object> objects = new List<Object>();

            foreach (var parameterObject in data)
            {
                if(parameterObject.GetType().IsArray || parameterObject.GetType().IsGenericType && parameterObject is IEnumerable)
                {
                    jarrays.Add(JArray.FromObject(parameterObject, _jsonSerializer));
                } else if(parameterObject.GetType().IsClass && !parameterObject.GetType().IsPrimitive && !(parameterObject is string))
                {
                    jobjects.Add(JObject.FromObject(parameterObject, _jsonSerializer));
                } else if(parameterObject.GetType().IsPrimitive || (parameterObject is string))
                {
                    objects.Add(parameterObject);
                }
            }

            if(jobjects.Count > 0 || jarrays.Count > 0)
            {
                BridgeConnector.Socket.Emit("sendToIpcRenderer", JObject.FromObject(browserWindow, _jsonSerializer), channel, jarrays.ToArray(), jobjects.ToArray(), objects.ToArray());
            }
            else
            {
                BridgeConnector.Socket.Emit("sendToIpcRenderer", JObject.FromObject(browserWindow, _jsonSerializer), channel, data);
            }
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}