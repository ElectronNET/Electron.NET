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
            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.electronProcess?.Stop();
            this.signalRFacade?.DisposeSocket();
            return Task.CompletedTask;
        }

        private void OnAspNetReady(object sender, EventArgs e)
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
            
            // Update the runtime port so WindowManager uses the correct URL
            ElectronNetRuntime.AspNetWebPort = this.port;
            
            this.LaunchElectron();
            this.electronLaunched = true;
        }

        private void LaunchElectron()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var flag = isUnPacked ? "--unpackeddotnetsignalr" : "--dotnetpackedsignalr";
            var args = $"{flag} --electronurl={this.actualUrl}";

            this.electronProcess = new ElectronProcessActive(isUnPacked, ElectronNetRuntime.ElectronExecutable, args, this.port.Value);
            // Note: We do NOT subscribe to electronProcess.Ready in SignalR mode.
            // The "ready" signal comes from the SignalR connection, not stdout.
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;
            _ = this.electronProcess.Start();
        }

        private async void SignalRFacade_Connected(object sender, EventArgs e)
        {
            // Register handler for 'electron-host-ready' signal from Electron.
            // This ensures API modules are fully loaded before calling the app ready callback.
            this.signalRFacade.Once("electron-host-ready", () =>
            {
                this.OnElectronHostReady();
            });
        }

        private async void OnElectronHostReady()
        {
            this.TransitionState(LifetimeState.Ready);
            
            // Execute the app ready callback
            if (ElectronNetRuntime.OnAppReadyCallback != null)
            {
                try
                {
                    await ElectronNetRuntime.OnAppReadyCallback().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Exception in app ready callback: {ex}");
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
