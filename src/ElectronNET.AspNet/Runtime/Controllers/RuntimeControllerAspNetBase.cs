namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;
    using ElectronNET.API;
    using ElectronNET.AspNet.Services;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.SocketBridge;

    internal abstract class RuntimeControllerAspNetBase : RuntimeControllerBase
    {
        private readonly IServer server;
        private readonly AspNetLifetimeAdapter aspNetLifetimeAdapter;
        private readonly IElectronAuthenticationService authenticationService;
        private SocketBridgeService socketBridge;

        protected RuntimeControllerAspNetBase(IServer server, AspNetLifetimeAdapter aspNetLifetimeAdapter, IElectronAuthenticationService authenticationService = null)
        {
            this.server = server;
            this.aspNetLifetimeAdapter = aspNetLifetimeAdapter;
            this.authenticationService = authenticationService;
            this.aspNetLifetimeAdapter.Ready += this.AspNetLifetimeAdapter_Ready;
            this.aspNetLifetimeAdapter.Stopping += this.AspNetLifetimeAdapter_Stopping;
            this.aspNetLifetimeAdapter.Stopped += this.AspNetLifetimeAdapter_Stopped;

            ElectronNetRuntime.RuntimeControllerCore = this;
        }

        internal override SocketBridgeService SocketBridge => this.socketBridge;

        internal override SocketIoFacade Socket
        {
            get
            {
                if (this.State == LifetimeState.Ready)
                {
                    return this.socketBridge.Socket;
                }

                throw new Exception("Cannot access socket bridge. Runtime is not in 'Ready' state");
            }
        }

        protected void CreateSocketBridge(int port, string authorization)
        {
            this.socketBridge = new SocketBridgeService(port, authorization);
            this.socketBridge.Ready += this.SocketBridge_Ready;
            this.socketBridge.Stopped += this.SocketBridge_Stopped;
            this.socketBridge.Start();
        }

        protected void HandleReady()
        {
            if (this.SocketBridge.IsReady() &&
                this.ElectronProcess.IsReady() &&
                this.aspNetLifetimeAdapter.IsReady())
            {
                var token = ElectronNetRuntime.ElectronAuthToken;
                var serverAddressesFeature = this.server.Features.Get<IServerAddressesFeature>();
                var address = serverAddressesFeature.Addresses.First();
                var uri = new Uri(address);

                // Only if somebody registered an IElectronAuthenticationService service - otherwise we do not care
                this.authenticationService?.SetExpectedToken(token);
                ElectronNetRuntime.AspNetWebPort = uri.Port;

                this.TransitionState(LifetimeState.Ready);
                Task.Run(this.RunReadyCallback);
            }
        }

        protected void HandleStopped()
        {
            this.TransitionState(LifetimeState.Stopping);

            if (this.SocketBridge.IsNotStopped())
            {
                this.SocketBridge.Stop();
            }

            if (this.ElectronProcess.IsNotStopped())
            {
                this.ElectronProcess.Stop();
            }

            if (this.aspNetLifetimeAdapter.IsNotStopped())
            {
                this.aspNetLifetimeAdapter.Stop();
            }

            if ((this.SocketBridge.IsNullOrStopped()) &&
                (this.ElectronProcess.IsNullOrStopped()) &&
                (this.aspNetLifetimeAdapter.IsNullOrStopped()))
            {
                this.TransitionState(LifetimeState.Stopped);
            }
        }

        protected abstract override Task StopCore();

        private void SocketBridge_Ready(object sender, EventArgs e)
        {
            this.HandleReady();
        }

        private void AspNetLifetimeAdapter_Ready(object sender, EventArgs e)
        {
            this.HandleReady();
        }

        private void SocketBridge_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void AspNetLifetimeAdapter_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void AspNetLifetimeAdapter_Stopping(object sender, EventArgs e)
        {
        }

        private async Task RunReadyCallback()
        {
            if (ElectronNetRuntime.OnAppReadyCallback == null)
            {
                Console.WriteLine("Warning: Non OnReadyCallback provided in UseElectron() setup.");
                return;
            }

            try
            {
                await ElectronNetRuntime.OnAppReadyCallback().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while executing OnAppReadyCallback. Stopping...\n" + ex);
                this.Stop();
            }
        }
    }
}
