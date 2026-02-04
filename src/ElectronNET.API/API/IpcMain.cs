namespace ElectronNET.API
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using ElectronNET.Serialization;

    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public sealed class IpcMain
    {
        private static IpcMain _ipcMain;
        private static object _syncRoot = new object();
        private static readonly JsonSerializerOptions BoxedObjectSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new JsonToBoxedPrimitivesConverter(),
            }
        };


        internal IpcMain()
        {
        }

        internal static IpcMain Instance
        {
            get
            {
                if (_ipcMain == null)
                {
                    lock (_syncRoot)
                    {
                        if (_ipcMain == null)
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
        public async Task On(string channel, Action<object> listener)
        {
            await BridgeConnector.Socket.Emit("registerIpcMainChannel", channel).ConfigureAwait(false);
            BridgeConnector.Socket.Off(channel);
            BridgeConnector.Socket.On<JsonElement>(channel, (args) =>
            {
                var arg = FormatArguments(args);
                listener(arg);
            });
        }

        private static object FormatArguments(JsonElement args)
        {
            var objectArray = args.Deserialize<object[]>(BoxedObjectSerializationOptions).ToList();

            Debug.Assert(objectArray.Count <= 2);

            if (objectArray.Count == 2)
            {
                return objectArray[1];
            }

            return null;
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
            BridgeConnector.Socket.On<JsonElement>(channel, (args) =>
            {
                var arg = FormatArguments(args);
                var result = listener(arg);
                BridgeConnector.Socket.Emit(channel + "Sync", result);
            });
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
        public void OnSync(string channel, Func<object, Task<object>> listener)
        {
            BridgeConnector.Socket.Emit("registerSyncIpcMainChannel", channel);
            BridgeConnector.Socket.On<JsonElement>(channel, (args) =>
            {
                Task.Run(async () =>
                {
                    var arg = FormatArguments(args);
                    var result = await listener(arg);
                    BridgeConnector.Socket.Emit(channel + "Sync", result);
                });
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
            BridgeConnector.Socket.Once<JsonElement>(channel, (args) =>
            {
                var arg = FormatArguments(args);
                listener(arg);
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
            BridgeConnector.Socket.Emit("sendToIpcRenderer", browserWindow, channel, data);
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
            BridgeConnector.Socket.Emit("sendToIpcRendererBrowserView", browserView.Id, channel, data);
        }

        /// <summary>
        /// Adds a handler for an invokeable IPC. This handler will be called
        /// whenever a renderer calls ipcRenderer.invoke(channel, ...args).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void Handle(string channel, Func<object, object> listener)
        {
            BridgeConnector.Socket.Emit("registerHandleIpcMainChannel", channel);
            BridgeConnector.Socket.On<JsonElement>(channel, (args) =>
            {
                var arg = FormatArguments(args);
                var result = listener(arg);
                BridgeConnector.Socket.Emit(channel + "Handle", result);
            });
        }

        /// <summary>
        /// Adds a handler for an invokeable IPC. This handler will be called
        /// whenever a renderer calls ipcRenderer.invoke(channel, ...args).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void Handle(string channel, Func<object, Task<object>> listener)
        {
            BridgeConnector.Socket.Emit("registerHandleIpcMainChannel", channel);
            BridgeConnector.Socket.On<JsonElement>(channel, (args) =>
            {
                Task.Run(async () =>
                {
                    var arg = FormatArguments(args);
                    var result = await listener(arg);
                    BridgeConnector.Socket.Emit(channel + "Handle", result);
                });
            });
        }

        /// <summary>
        /// Handles a single invokeable IPC message, then removes the listener.
        /// See ipcMain.handle(channel, listener).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void HandleOnce(string channel, Func<object, object> listener)
        {
            BridgeConnector.Socket.Emit("registerHandleOnceIpcMainChannel", channel);
            BridgeConnector.Socket.Once<JsonElement>(channel, (args) =>
            {
                var arg = FormatArguments(args);
                var result = listener(arg);
                BridgeConnector.Socket.Emit(channel + "HandleOnce", result);
            });
        }

        /// <summary>
        /// Handles a single invokeable IPC message, then removes the listener.
        /// See ipcMain.handle(channel, listener).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void HandleOnce(string channel, Func<object, Task<object>> listener)
        {
            BridgeConnector.Socket.Emit("registerHandleOnceIpcMainChannel", channel);
            BridgeConnector.Socket.Once<JsonElement>(channel, (args) =>
            {
                Task.Run(async () =>
                {
                    var arg = FormatArguments(args);
                    var result = await listener(arg);
                    BridgeConnector.Socket.Emit(channel + "HandleOnce", result);
                });
            });
        }

        /// <summary>
        /// Removes any handler for channel, if present.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        public void RemoveHandler(string channel)
        {
            BridgeConnector.Socket.Emit("removeHandlerIpcMainChannel", channel);
        }
    }
}