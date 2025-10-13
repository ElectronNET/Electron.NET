namespace ElectronNET.Runtime.Services.SocketBridge
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using ElectronNET.Runtime.Data;

    internal  class SocketBridgeService : LifetimeServiceBase
    {
        private readonly int socketPort;
        private readonly string socketUrl;
        private SocketIoFacade socket;

        public SocketBridgeService(int socketPort)
        {
            this.socketPort = socketPort;
            this.socketUrl = $"http://localhost:{this.socketPort}";
        }

        public int SocketPort => this.socketPort;

        internal SocketIoFacade Socket => this.socket;

        protected override Task StartCore()
        {
            this.socket = new SocketIoFacade(this.socketUrl);
            this.socket.BridgeConnected += this.Socket_BridgeConnected;
            this.socket.BridgeDisconnected += this.Socket_BridgeDisconnected;
            Task.Run(this.Connect);

            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.socket.DisposeSocket();
            return Task.CompletedTask;
        }

        private void Connect()
        {
            this.socket.Connect();
            this.TransitionState(LifetimeState.Started);
        }

        private void Socket_BridgeDisconnected(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Stopped);
        }

        private void Socket_BridgeConnected(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Ready);
        }
    }
}
