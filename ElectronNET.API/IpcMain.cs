using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public sealed class IpcMain : IIpcMain
    {
        private static IpcMain _ipcMain;
        private static readonly object _syncRoot = new();

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

        public static bool IsConnected => BridgeConnector.IsConnected;

        /// <summary>
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void On(string channel, Action<object> listener)
        {
            BridgeConnector.Emit("registerIpcMainChannel", channel);
            BridgeConnector.Off(channel);
            BridgeConnector.On<object[]>(channel, (args) =>
            {
                var objectArray = FormatArguments(args);

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
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...). This listner will keep the window event sender id
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void OnWithId(string channel, Action<(int browserId, int webContentId, object arguments)> listener)
        {
            BridgeConnector.Emit("registerIpcMainChannelWithId", channel);
            BridgeConnector.Off(channel);
            BridgeConnector.On<ArgsAndIds>(channel, (data) =>
            {
                var objectArray = FormatArguments(data.args);

                if (objectArray.Count == 1)
                {
                    listener((data.id, data.wcId, objectArray.First()));
                }
                else
                {
                    listener((data.id, data.wcId, objectArray));
                }
            });
        }

        private class ArgsAndIds
        {
            public int  id { get; set; }
            public int wcId { get; set; }
            public object[] args { get; set; }
        }

        private List<object> FormatArguments(object[] objectArray)
        {
            return objectArray.Where(o => o is object).ToList();
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
            BridgeConnector.Emit("registerSyncIpcMainChannel", channel);
            BridgeConnector.On<object[]>(channel, (args) => {
                var objectArray = FormatArguments(args);
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
                BridgeConnector.Emit(channel + "Sync", result);
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
            BridgeConnector.Emit("registerOnceIpcMainChannel", channel);
            BridgeConnector.Once<object[]>(channel, (args) =>
            {
                var objectArray = FormatArguments(args);

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
            BridgeConnector.Emit("removeAllListenersIpcMainChannel", channel);
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
            var objectsWithCorrectSerialization = new List<object>
            {
                browserWindow.Id,
                channel
            };

            foreach (var parameterObject in data)
            {
                if(parameterObject.GetType().IsArray || parameterObject.GetType().IsGenericType && parameterObject is IEnumerable)
                {
                    objectsWithCorrectSerialization.Add(JArray.FromObject(parameterObject, _jsonSerializer));
                } 
                else if(parameterObject.GetType().IsClass && !parameterObject.GetType().IsPrimitive && !(parameterObject is string))
                {
                    objectsWithCorrectSerialization.Add(JObject.FromObject(parameterObject, _jsonSerializer));
                } 
                else if(parameterObject.GetType().IsPrimitive || (parameterObject is string))
                {
                    objectsWithCorrectSerialization.Add(parameterObject);
                }
            }

            BridgeConnector.Emit("sendToIpcRenderer", objectsWithCorrectSerialization.ToArray());
        }

        /// <summary>
        /// Send a message to the BrowserView renderer process asynchronously via channel, you can also send
        /// arbitrary arguments. Arguments will be serialized in JSON internally and hence
        /// no functions or prototype chain will be included. The renderer process handles it by
        /// listening for channel with ipcRenderer module.
        /// </summary>
        /// <param name="browserView">BrowserView with channel.</param>
        /// <param name="channel">Channelname.</param>
        /// <param name="data">Arguments data.</param>
        public void Send(BrowserView browserView, string channel, params object[] data)
        {
            List<JObject> jobjects = new();
            List<JArray> jarrays = new();
            List<object> objects = new();

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
                BridgeConnector.Emit("sendToIpcRendererBrowserView", browserView.Id, channel, jarrays.ToArray(), jobjects.ToArray(), objects.ToArray());
            }
            else
            {
                BridgeConnector.Emit("sendToIpcRendererBrowserView", browserView.Id, channel, data);
            }
        }

        /// <summary>
        /// Log a message to the console output pipe. This is used when running with "detachedProcess" : true on the electron.manifest.json,
        /// as in that case we can't open pipes to read the console output from the child process anymore
        /// </summary>
        /// <param name="text">Message to log</param>
        public static void ConsoleLog(string text)
        {
            BridgeConnector.Emit("console-stdout", text);
        }

        /// <summary>
        /// Log a message to the console error pipe. This is used when running with "detachedProcess" : true on the electron.manifest.json,
        /// as in that case we can't open pipes to read the console output from the child process anymore
        /// </summary>
        /// <param name="text">Message to log</param>

        public static void ConsoleError(string text)
        {
            BridgeConnector.Emit("console-stderr", text);
        }

        private readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}