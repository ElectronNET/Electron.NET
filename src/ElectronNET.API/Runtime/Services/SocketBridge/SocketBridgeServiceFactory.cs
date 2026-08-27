namespace ElectronNET.Runtime.Services.SocketBridge
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class SocketBridgeServiceFactory : ISocketBridgeServiceFactory
    {
        private readonly Action<IServiceCollection> configureSocketIO;

        public SocketBridgeServiceFactory(Action<IServiceCollection> configureSocketIO)
        {
            this.configureSocketIO = configureSocketIO;
        }

        public SocketBridgeService Create(int socketPort, string authorization)
        {
            return new SocketBridgeService(socketPort, authorization, this.configureSocketIO);
        }
    }
}
