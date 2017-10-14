using System;

namespace ElectronNET.API
{
    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public sealed class IpcMain
    {
        private static IpcMain _ipcMain;

        private IpcMain() { }

        public static IpcMain Instance
        {
            get
            {
                if(_ipcMain == null)
                {
                    _ipcMain = new IpcMain();
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
            BridgeConnector.Socket.On(channel, listener);
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
            BridgeConnector.Socket.On(channel, listener);
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
        /// <param name="channel">Channelname.</param>
        /// <param name="data">Arguments data.</param>
        public void Send(string channel, params object[] data)
        {
            BridgeConnector.Socket.Emit("sendToIpcRenderer", channel, data);
        }
    }
}