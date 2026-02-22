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
    using ElectronNET.AspNet.Services;

    /// <summary>
    /// Runtime controller for SignalR-based .NET-first startup mode.
    /// Key differences from Socket.IO mode:
    /// - Waits for ASP.NET server to start, then captures the dynamic port
    /// - Launches Electron with the actual URL (no port scanning needed)
    /// - Uses SignalRConnection instead of SocketIOConnection for bidirectional communication
    /// - Waits for 'electron-host-ready' signal to ensure API modules are loaded before calling app callback
    /// </summary>
    internal class RuntimeControllerAspNetDotnetFirstSignalR : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private readonly IServer server;
        private readonly IHubContext<ElectronHub> hubContext;
        private readonly IElectronAuthenticationService authenticationService;
        private SignalRConnection socket;
        private int? port;
        private string actualUrl;
        private bool electronLaunched;
        private string authenticationToken;

        public RuntimeControllerAspNetDotnetFirstSignalR(
            AspNetLifetimeAdapter aspNetLifetimeAdapter, 
            IServer server,
            IHubContext<ElectronHub> hubContext,
            IElectronAuthenticationService authenticationService) 
            : base(aspNetLifetimeAdapter)
        {
            this.server = server;
            this.hubContext = hubContext;
            this.authenticationService = authenticationService;
            this.socket = new SignalRConnection(hubContext);
            this.electronLaunched = false;
            
            this.socket.BridgeConnected += this.SignalRConnection_Connected;
            this.socket.BridgeDisconnected += this.SignalRConnection_Disconnected;
            
            // Subscribe to ASP.NET ready event to launch Electron
            aspNetLifetimeAdapter.Ready += this.OnAspNetReady;
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;
        internal override SocketBridgeService SocketBridge => null;
        
        internal override ISocketConnection Socket
        {
            get
            {
                if (this.State == LifetimeState.Ready)
                {
                    return this.socket;
                }

                throw new Exception("Cannot access SignalR facade. Runtime is not in 'Ready' state");
            }
        }

        internal SignalRConnection SignalRSocket => this.socket;

        protected override Task StartCore()
        {
            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.electronProcess?.Stop();
            this.socket?.Dispose();
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
            // Generate secure authentication token (128-bit cryptographic random GUID)
            // This token protects against unauthorized connections from other users on the same machine
            this.authenticationToken = Guid.NewGuid().ToString("N"); // 32 hex chars, no hyphens
            
            // Register token with authentication service for validation
            // The middleware will validate this token on all HTTP and SignalR requests
            this.authenticationService.SetExpectedToken(this.authenticationToken);
            
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var flag = isUnPacked ? "--unpackeddotnetsignalr" : "--dotnetpackedsignalr";
            var args = $"{flag} --electronurl={this.actualUrl} --authtoken={this.authenticationToken}";

            this.electronProcess = new ElectronProcessActive(isUnPacked, ElectronNetRuntime.ElectronExecutable, args, this.port.Value);
            // Note: We do NOT subscribe to electronProcess.Ready in SignalR mode.
            // The "ready" signal comes from the SignalR connection, not stdout.
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;
            _ = this.electronProcess.Start();
        }

        private async void SignalRConnection_Connected(object sender, EventArgs e)
        {
            // Register handler for 'electron-host-ready' signal from Electron.
            // This ensures API modules are fully loaded before calling the app ready callback.
            this.socket.Once("electron-host-ready", () =>
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

        private void SignalRConnection_Disconnected(object sender, EventArgs e)
        {
            // IMPORTANT: Do NOT call HandleStopped synchronously here!
            // This event fires from within SignalR's OnDisconnectedAsync, and calling
            // StopApplication() synchronously causes a deadlock: the host waits for
            // OnDisconnectedAsync to complete, but we're waiting for the host to stop.
            // Fire and forget to break the deadlock.
            _ = Task.Run(() => this.HandleStopped());
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        public void OnSignalRConnected(string connectionId)
        {
            this.socket.SetConnectionId(connectionId);
        }

        public void OnSignalRDisconnected()
        {
            this.socket.OnDisconnected();
        }
    }
}
