namespace ElectronNET.Runtime.Services.SocketBridge
{
    using ElectronNET.API;
    using ElectronNET.Runtime.Data;
    using System;
    using System.Threading.Tasks;

    internal class SocketBridgeService : LifetimeServiceBase
    {
        private readonly int socketPort;
        private readonly string authorization;
        private readonly string socketUrl;
        private SocketIOConnection socket;

        public SocketBridgeService(int socketPort, string authorization)
        {
            this.socketPort = socketPort;
            this.authorization = authorization;
            this.socketUrl = $"http://{FormatHost(ElectronNetRuntime.ElectronSocketHost)}:{this.socketPort}";
        }

        // The Electron host reports the loopback address it is actually listening on; only
        // when it is unknown we have to fall back to the ambiguous hostname.
        private static string FormatHost(string socketHost)
        {
            if (string.IsNullOrWhiteSpace(socketHost))
            {
                return "localhost";
            }

            return socketHost.Contains(':') ? $"[{socketHost}]" : socketHost;
        }

        public int SocketPort => this.socketPort;

        internal SocketIOConnection Socket => this.socket;

        protected override Task StartCore()
        {
            this.socket = new SocketIOConnection(this.socketUrl, this.authorization);
            this.socket.BridgeConnected += this.Socket_BridgeConnected;
            this.socket.BridgeDisconnected += this.Socket_BridgeDisconnected;
            Task.Run(this.Connect);

            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.socket.Dispose();
            return Task.CompletedTask;
        }

        private void Connect()
        {
            this.socket.Connect();
            if (this.State < LifetimeState.Started)
            {
                this.TransitionState(LifetimeState.Started);
            }
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