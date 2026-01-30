namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;

    internal class RuntimeControllerAspNetDotnetFirstSignalR : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private readonly IServer server;
        private int? port;
        private string actualUrl;
        private bool electronLaunched;

        public RuntimeControllerAspNetDotnetFirstSignalR(AspNetLifetimeAdapter aspNetLifetimeAdapter, IServer server) 
            : base(aspNetLifetimeAdapter)
        {
            this.server = server;
            this.electronLaunched = false;
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

        protected override Task StartCore()
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] StartCore - ASP.NET starting, will launch Electron when ready");
            // We wait for ASP.NET to become ready via the AspNetLifetimeAdapter
            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.electronProcess?.Stop();
            return Task.CompletedTask;
        }

        // Called by base class when ASP.NET becomes ready
        protected override void HandleReady()
        {
            // If Electron hasn't been launched yet and ASP.NET is ready, launch it
            if (!this.electronLaunched)
            {
                this.CapturePortAndLaunchElectron();
            }
            
            // Don't call base.HandleReady() yet - we need to wait for Electron and SignalR
        }

        private void CapturePortAndLaunchElectron()
        {
            // Capture the actual port from Kestrel
            var addresses = this.server.Features.Get<IServerAddressesFeature>();
            if (addresses == null || !addresses.Addresses.Any())
            {
                Console.Error.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] ERROR: Could not retrieve server addresses");
                throw new Exception("Could not retrieve server addresses from Kestrel");
            }

            // Get the first address (should be http://localhost:XXXXX)
            this.actualUrl = addresses.Addresses.First();
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Kestrel listening on: {this.actualUrl}");

            // Parse the port from the URL
            var uri = new Uri(this.actualUrl);
            this.port = uri.Port;
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Captured port: {this.port}");

            // Launch Electron process
            this.LaunchElectron();
            this.electronLaunched = true;
        }

        private void LaunchElectron()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var electronBinaryName = ElectronNetRuntime.ElectronExecutable;
            
            // Build command line arguments including the URL and startup mode flag
            var startupModeFlag = isUnPacked ? "--unpackeddotnetsignalr" : "--dotnetpackedsignalr";
            var args = $"{startupModeFlag} --electronUrl={this.actualUrl}";
            
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] Launching Electron...");
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] isUnPacked: {isUnPacked}");
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] electronBinaryName: {electronBinaryName}");
            Console.WriteLine($"[RuntimeControllerAspNetDotnetFirstSignalR] args: {args}");

            this.electronProcess = new ElectronProcessActive(isUnPacked, electronBinaryName, args, this.port.Value);
            this.electronProcess.Ready += this.ElectronProcess_Ready;
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;

            _ = this.electronProcess.Start();
        }

        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Electron process ready - waiting for SignalR connection");
            this.TransitionState(LifetimeState.Started);
            
            // TODO: Wait for SignalR connection from ElectronHub before transitioning to Ready
            // For now, assume connection happens quickly
            Task.Delay(2000).ContinueWith(_ => 
            {
                if (this.State == LifetimeState.Started)
                {
                    Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Transitioning to Ready (SignalR assumed connected)");
                    this.TransitionState(LifetimeState.Ready);
                }
            });
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            Console.WriteLine("[RuntimeControllerAspNetDotnetFirstSignalR] Electron process stopped");
            this.HandleStopped();
        }
    }
}
