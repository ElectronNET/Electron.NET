namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using ElectronNET.Runtime.Services.SocketBridge;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.SignalR;
    using ElectronNET.AspNet.Hubs;

    internal class RuntimeControllerAspNetDotnetFirstSignalR : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private readonly IServer server;
        private readonly IHubContext<ElectronHub> hubContext;
        private SignalRFacade signalRFacade;
        private int? port;
        private string actualUrl;
        private bool electronLaunched;

        public RuntimeControllerAspNetDotnetFirstSignalR(
            AspNetLifetimeAdapter aspNetLifetimeAdapter, 
            IServer server,
            IHubContext<ElectronHub> hubContext) 
            : base(aspNetLifetimeAdapter)
        {
            this.server = server;
            this.hubContext = hubContext;
            this.signalRFacade = new SignalRFacade(hubContext);
            this.electronLaunched = false;
            
            this.signalRFacade.BridgeConnected += this.SignalRFacade_Connected;
            this.signalRFacade.BridgeDisconnected += this.SignalRFacade_Disconnected;
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;
        internal override SocketBridgeService SocketBridge => null;
        
        internal override SocketIoFacade Socket
        {
            get
            {
                throw new NotImplementedException("SignalR mode uses SignalRFacade");
            }
        }

        internal SignalRFacade SignalRSocket => this.signalRFacade;

        protected override Task StartCore()
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] StartCore");
            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.electronProcess?.Stop();
            this.signalRFacade?.DisposeSocket();
            return Task.CompletedTask;
        }

        protected override void HandleReady()
        {
            if (!this.electronLaunched)
            {
                this.CapturePortAndLaunchElectron();
            }
        }

        private void CapturePortAndLaunchElectron()
        {
            var addresses = this.server.Features.Get<IServerAddressesFeature>();
            if (addresses == null || !addresses.Addresses.Any())
            {
                throw new Exception("Could not retrieve server addresses");
            }

            this.actualUrl = addresses.Addresses.First();
            this.port = new Uri(this.actualUrl).Port;
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] URL: {this.actualUrl}");
            
            this.LaunchElectron();
            this.electronLaunched = true;
        }

        private void LaunchElectron()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var flag = isUnPacked ? "--unpackeddotnetsignalr" : "--dotnetpackedsignalr";
            var args = $"{flag} --electronUrl={this.actualUrl}";
            
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Launching: {args}");

            this.electronProcess = new ElectronProcessActive(isUnPacked, ElectronNetRuntime.ElectronExecutable, args, this.port.Value);
            this.electronProcess.Ready += this.ElectronProcess_Ready;
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;
            _ = this.electronProcess.Start();
        }

        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Electron ready");
            this.TransitionState(LifetimeState.Started);
        }

        private void SignalRFacade_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] SignalR connected!");
            this.TransitionState(LifetimeState.Ready);
        }

        private void SignalRFacade_Disconnected(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        public void OnSignalRConnected(string connectionId)
        {
            this.signalRFacade.SetConnectionId(connectionId);
        }

        public void OnSignalRDisconnected()
        {
            this.signalRFacade.OnDisconnected();
        }
    }
}
