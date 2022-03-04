using ElectronNET.API.Hubs;
using ElectronNET.API.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace ElectronNET.API
{

    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public sealed class IpcMain
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

        /// <summary>
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public async void On(string channel, Action<object> listener)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("registerIpcMainChannel", channel);

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(Electron.SignalrObservedJArray, "CollectionChanged")
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Subscribe(x => {
                    if (x.EventArgs.NewItems != null)
                    {
                        foreach (SignalrResponse entry in x.EventArgs.NewItems)
                        {
                            if (entry.Channel == channel && entry.Value != null)
                            {
                                List<object> objectArray = FormatArguments(entry.Value);

                                if (objectArray.Count == 1)
                                {
                                    listener(objectArray.First());
                                }
                                else
                                {
                                    listener(objectArray);
                                }
                            }
                        }
                    }
                });

        }

        /// <summary>
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...). This listner will keep the window event sender id
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public async void OnWithId(string channel, Action<(int browserId, int webContentId, object arguments)> listener)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("registerIpcMainChannelWithId", channel);

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(Electron.SignalrObservedJObject, "CollectionChanged")
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Subscribe(x => {
                    if (x.EventArgs.NewItems != null)
                    {
                        foreach (SignalrResponseJObject entry in x.EventArgs.NewItems)
                        {
                            //ArgsAndIds signalrResponse = ((JObject)args).ToObject<ArgsAndIds>();
                            if (entry.Channel == channel && entry.Value != null)
                            {
                                var signalrResponse = ((JObject)entry.Value).ToObject<ArgsAndIds>();
                                List<object> objectArray = FormatArgumentsIds(signalrResponse.args);

                                if (objectArray.Count == 1)
                                {
                                    listener((signalrResponse.id, signalrResponse.wcId, objectArray.First()));
                                }
                                else
                                {
                                    listener((signalrResponse.id, signalrResponse.wcId, objectArray));
                                }
                            }
                        }
                    }
                });
        }

        private class ArgsAndIds
        {
            public int id { get; set; }
            public int wcId { get; set; }
            public object[] args { get; set; }
        }

        private List<object> FormatArgumentsIds(object[] objectArray)
        {
            return objectArray.Where(o => o is object).ToList();
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
        public async void OnSync(string channel, Func<object, object> listener)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("registerSyncIpcMainChannel", channel);

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(Electron.SignalrObservedJArray, "CollectionChanged")
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Subscribe(x => {
                    if (x.EventArgs.NewItems != null)
                    {
                        foreach (SignalrResponse entry in x.EventArgs.NewItems)
                        {
                            if (entry.Channel == channel && entry.Value != null)
                            {
                                List<object> objectArray = FormatArguments(entry.Value);
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
                                Electron.SignalrElectron.Clients.All.SendAsync(channel + "Sync", result);
                            }
                        }
                    }
                });            
        }

        /// <summary>
        /// Adds a one time listener method for the event. This listener is invoked only
        ///  the next time a message is sent to channel, after which it is removed.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public async void Once(string channel, Action<object> listener)
        {
            var resultSignalr = await SignalrSerializeHelper.GetSignalrResultJArrayNoTimeout("registerOnceIpcMainChannel", channel);

            List<object> objectArray = FormatArguments(resultSignalr);

            if (objectArray.Count == 1)
            {
                listener(objectArray.First());
            }
            else
            {
                listener(objectArray);
            }

        }

        /// <summary>
        /// Removes listeners of the specified channel.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        public void RemoveAllListeners(string channel)
        {
            Electron.SignalrElectron.Clients.All.SendAsync("removeAllListenersIpcMainChannel", channel);
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
            List<object> objects = new List<object>();

            foreach (var parameterObject in data)
            {
                if (parameterObject.GetType().IsArray || parameterObject.GetType().IsGenericType && parameterObject is IEnumerable)
                {
                    jarrays.Add(JArray.FromObject(parameterObject, _jsonSerializer));
                }
                else if (parameterObject.GetType().IsClass && !parameterObject.GetType().IsPrimitive && !(parameterObject is string))
                {
                    jobjects.Add(JObject.FromObject(parameterObject, _jsonSerializer));
                }
                else if (parameterObject.GetType().IsPrimitive || (parameterObject is string))
                {
                    objects.Add(parameterObject);
                }
            }

            if (jobjects.Count > 0 || jarrays.Count > 0)
            {
                Electron.SignalrElectron.Clients.All.SendAsync("sendToIpcRenderer", JObject.FromObject(browserWindow, _jsonSerializer), channel, jarrays.ToArray(), jobjects.ToArray(), objects.ToArray());
            }
            else
            {
                Electron.SignalrElectron.Clients.All.SendAsync("sendToIpcRenderer", JObject.FromObject(browserWindow, _jsonSerializer), channel, data);
            }
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

            if (jobjects.Count > 0 || jarrays.Count > 0)
            {
                Electron.SignalrElectron.Clients.All.SendAsync("sendToIpcRendererBrowserView", browserView.Id, channel, jarrays.ToArray(), jobjects.ToArray(), objects.ToArray());
            }
            else
            {
                Electron.SignalrElectron.Clients.All.SendAsync("sendToIpcRendererBrowserView", browserView.Id, channel, data);
            }
        }

        /// <summary>
        /// Log a message to the console output pipe. This is used when running with "detachedProcess" : true on the electron.manifest.json,
        /// as in that case we can't open pipes to read the console output from the child process anymore
        /// </summary>
        /// <param name="text">Message to log</param>
        public static async void ConsoleLog(string text)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("console-stdout", text);
        }

        /// <summary>
        /// Log a message to the console error pipe. This is used when running with "detachedProcess" : true on the electron.manifest.json,
        /// as in that case we can't open pipes to read the console output from the child process anymore
        /// </summary>
        /// <param name="text">Message to log</param>

        public static async void ConsoleError(string text)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("console-stderr", text);
        }

        private readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}