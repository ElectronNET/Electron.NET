using ElectronNET.API.Interfaces;
using SocketIOClient;

namespace ElectronNET.API
{
    /// <summary>
    /// Wrapper for the underlying Socket connection
    /// </summary>
    public class ApplicationSocket : IApplicationSocket
    {
        /// <summary>
        /// Socket used to communicate with main.js
        /// </summary>
        public SocketIO Socket { get; internal set; }
    }
}
