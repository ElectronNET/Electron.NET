using Quobject.SocketIoClientDotNet.Client;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Wrapper for the underlying Socket connection
    /// </summary>
    public interface IApplicationSocket
    {
        /// <summary>
        /// Socket used to communicate with main.js
        /// </summary>
        Socket Socket { get; }
    }
}