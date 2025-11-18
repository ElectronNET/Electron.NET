namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET;
    using ElectronNET.API;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.SocketBridge;

    internal abstract class RuntimeControllerAspNetBase : RuntimeControllerBase
    {
        private readonly AspNetLifetimeAdapter aspNetLifetimeAdapter;
        private SocketBridgeService socketBridge;

        protected RuntimeControllerAspNetBase(AspNetLifetimeAdapter aspNetLifetimeAdapter)
        {
            this.aspNetLifetimeAdapter = aspNetLifetimeAdapter;
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

        protected void CreateSocketBridge(int port)
        {
            this.socketBridge = new SocketBridgeService(port);
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

                // Everything is fully stopped – fire the OnQuit callback.
                Task.Run(this.RunQuitCallback);
            }
        }

        /// <summary>
        /// Invoked when ASP.NET lifetime enters Stopping (ApplicationStopping).
        /// We only trigger the OnWillQuit callback here; the actual state
        /// transition to Stopping is handled in <see cref="HandleStopped"/>.
        /// </summary>
        protected void HandleStopping()
        {
            Task.Run(this.RunWillQuitCallback);
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
            this.HandleStopping();
        }

        private async Task RunWillQuitCallback()
        {
            var events = ElectronNetRuntime.Options?.Events;
            var handler = events?.OnWillQuit;

            if (handler == null)
            {
                return;
            }

            try
            {
                await handler().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while executing OnWillQuit callback.\n" + ex);
                // We are already stopping; no need to call this.Stop() here.
            }
        }

        private async Task RunQuitCallback()
        {
            var events = ElectronNetRuntime.Options?.Events;
            var handler = events?.OnQuit;

            if (handler == null)
            {
                return;
            }

            try
            {
                await handler().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while executing OnQuit callback.\n" + ex);
            }
        }

        private async Task RunReadyCallback()
        {
            var events = ElectronNetRuntime.Options?.Events;
            if (events?.OnBeforeReady != null)
            {
                try
                {
                    await events.OnBeforeReady().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while executing OnBeforeReady callback. Stopping...\n" + ex);
                    this.Stop();
                    return;
                }
            }

            if (events.OnReady == null)
            {
                Console.WriteLine("Warning: Non OnReadyCallback provided in UseElectron() setup.");
                return;
            }

            try
            {
                await events.OnReady().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while executing OnAppReadyCallback. Stopping...\n" + ex);
                this.Stop();
            }
        }
    }
}
