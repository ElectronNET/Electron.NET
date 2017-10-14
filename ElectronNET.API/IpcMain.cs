using System;
using Quobject.SocketIoClientDotNet.Client;

namespace ElectronNET.API
{
    /// <summary>
    /// Communicate asynchronously from the main process to renderer processes.
    /// </summary>
    public class IpcMain
    {
        private Socket _socket;

        public IpcMain(Socket socket)
        {
            _socket = socket;
        }
 
        /// <summary>
        ///  Listens to channel, when a new message arrives listener would be called with 
        ///  listener(event, args...).
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void On(string channel, Action<object> listener)
        {
            _socket.Emit("registerIpcMainChannel", channel);
            _socket.On(channel, listener);
        }

        /// <summary>
        /// Adds a one time listener method for the event. This listener is invoked only
        ///  the next time a message is sent to channel, after which it is removed.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        /// <param name="listener">Callback Method.</param>
        public void Once(string channel, Action<object> listener)
        {
            _socket.Emit("registerOnceIpcMainChannel", channel);
            _socket.On(channel, listener);
        }

        /// <summary>
        /// Removes listeners of the specified channel.
        /// </summary>
        /// <param name="channel">Channelname.</param>
        public void RemoveAllListeners(string channel)
        {
            _socket.Emit("removeAllListenersIpcMainChannel", channel);
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
            _socket.Emit("sendToIpcRenderer", channel, data);
        }
    }
}