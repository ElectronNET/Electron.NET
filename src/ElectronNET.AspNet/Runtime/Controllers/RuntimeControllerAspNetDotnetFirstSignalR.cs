namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using ElectronNET.API.Bridge;
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
            
            // Subscribe to ASP.NET ready event to launch Electron
            aspNetLifetimeAdapter.Ready += this.OnAspNetReady;
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;
        internal override SocketBridgeService SocketBridge => null;
        
        internal override IFacade Socket
        {
            get
            {
                if (this.State == LifetimeState.Ready)
                {
                    return this.signalRFacade;
                }

                throw new Exception("Cannot access SignalR facade. Runtime is not in 'Ready' state");
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
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] StopCore called!");
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Stack trace: {Environment.StackTrace}");
            this.electronProcess?.Stop();
            this.signalRFacade?.DisposeSocket();
            return Task.CompletedTask;
        }

        private void OnAspNetReady(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] ASP.NET Ready - launching Electron");
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
            var args = $"{flag} --electronurl={this.actualUrl}";
            
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Launching: {args}");

            this.electronProcess = new ElectronProcessActive(isUnPacked, ElectronNetRuntime.ElectronExecutable, args, this.port.Value);
            // Note: We do NOT subscribe to electronProcess.Ready in SignalR mode
            // The "ready" signal comes from SignalR connection, not stdout
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;
            _ = this.electronProcess.Start();
        }

        // Keep this method but it won't be called in SignalR mode
        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
            // Not used in SignalR mode - ready state comes from SignalR connection
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Electron process started (stdout ready)");
        }

        private async void SignalRFacade_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] SignalR connected!");
            this.TransitionState(LifetimeState.Ready);
            
            // Execute the app ready callback
            if (ElectronNetRuntime.OnAppReadyCallback != null)
            {
                Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Executing app ready callback");
                try
                {
                    await ElectronNetRuntime.OnAppReadyCallback().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Exception in app ready callback: {ex}");
                    this.Stop();
                }
            }
        }

        private void SignalRFacade_Disconnected(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] ElectronProcess_Stopped event fired!");
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
