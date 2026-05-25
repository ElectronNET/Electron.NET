namespace ElectronNET.API;

using System;
using System.Threading.Tasks;

/// <summary>
/// Common interface for communication facades.
/// Provides methods for bidirectional communication between .NET and Electron.
/// </summary>
internal interface ISocketConnection : IDisposable
{
    /// <summary>
    /// Raised when the bridge connection is established.
    /// </summary>
    event EventHandler BridgeConnected;

    /// <summary>
    /// Raised when the bridge connection is lost.
    /// </summary>
    event EventHandler BridgeDisconnected;

    /// <summary>
    /// Establishes the connection to Electron.
    /// </summary>
    void Connect();

    /// <summary>
    /// Registers a persistent event handler.
    /// </summary>
    void On(string eventName, Action action);

    /// <summary>
    /// Registers a persistent event handler with a typed parameter.
    /// </summary>
    void On<T>(string eventName, Action<T> action);

    /// <summary>
    /// Registers a one-time event handler.
    /// </summary>
    void Once(string eventName, Action action);

    /// <summary>
    /// Registers a one-time event handler with a typed parameter.
    /// </summary>
    void Once<T>(string eventName, Action<T> action);

    /// <summary>
    /// Removes an event handler.
    /// </summary>
    void Off(string eventName);

    /// <summary>
    /// Sends a message to Electron.
    /// </summary>
    Task Emit(string eventName, params object[] args);
}