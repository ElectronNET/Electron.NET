namespace ElectronNET.Runtime.Services.SocketBridge
{
    /// <summary>
    /// Creates <see cref="SocketBridgeService"/> instances for a given runtime-only port/authorization
    /// pair. Constructor-injecting this (instead of calling `new SocketBridgeService(...)` directly)
    /// is the seam that lets a host application's SocketIOClient customizations reach the bridge.
    /// </summary>
    internal interface ISocketBridgeServiceFactory
    {
        SocketBridgeService Create(int socketPort, string authorization);
    }
}
