namespace ElectronNET.Runtime.Controllers
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using ElectronNET.Runtime.Services.SocketBridge;

    internal class RuntimeControllerDotNetFirst : RuntimeControllerBase
    {
        private ElectronProcessBase electronProcess;
        private SocketBridgeService socketBridge;
        private int? port;

        public RuntimeControllerDotNetFirst()
        {
        }

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

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

        internal override SocketBridgeService SocketBridge => this.socketBridge;

        protected override Task StartCore()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var electronBinaryName = ElectronNetRuntime.ElectronExecutable;
            var args = Environment.CommandLine;
            this.port = ElectronNetRuntime.ElectronSocketPort;

            if (!this.port.HasValue)
            {
                this.port = PortHelper.GetFreePort(ElectronNetRuntime.DefaultSocketPort);
                ElectronNetRuntime.ElectronSocketPort = this.port;
            }

            this.electronProcess = new ElectronProcessActive(isUnPacked, electronBinaryName, args, this.port.Value);
            this.electronProcess.Ready += this.ElectronProcess_Ready;
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;

            return this.electronProcess.Start();
        }

        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Started);
            this.socketBridge = new SocketBridgeService(this.port!.Value);
            this.socketBridge.Ready += this.SocketBridge_Ready;
            this.socketBridge.Stopped += this.SocketBridge_Stopped;
            this.socketBridge.Start();
        }

        private void SocketBridge_Ready(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Ready);

        }

        private void SocketBridge_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }

        private void HandleStopped()
        {
            if (this.socketBridge.State != LifetimeState.Stopped)
            {
                this.socketBridge.Stop();
            }
            else if (this.electronProcess.State != LifetimeState.Stopped)
            {
                this.electronProcess.Stop();
            }
            else
            {
                this.TransitionState(LifetimeState.Stopped);
            }
        }

        protected override Task StopCore()
        {
            this.electronProcess.Stop();
            return Task.CompletedTask;
        }

    }
}
