namespace ElectronNET.Runtime.Controllers
{
    using ElectronNET.API;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using ElectronNET.Runtime.Services.SocketBridge;
    using System;
    using System.Threading.Tasks;

    internal class RuntimeControllerElectronFirst : RuntimeControllerBase
    {
        private ElectronProcessBase electronProcess;
        private SocketBridgeService socketBridge;
        private int? port;

        public RuntimeControllerElectronFirst()
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
            this.port = ElectronNetRuntime.ElectronSocketPort;

            if (!this.port.HasValue)
            {
                throw new Exception("No port has been specified by Electron!");
            }

            if (!ElectronNetRuntime.ElectronProcessId.HasValue)
            {
                throw new Exception("No electronPID has been specified by Electron!");
            }

            this.TransitionState(LifetimeState.Starting);
            this.socketBridge = new SocketBridgeService(this.port!.Value);
            this.socketBridge.Ready += this.SocketBridge_Ready;
            this.socketBridge.Stopped += this.SocketBridge_Stopped;
            this.socketBridge.Start();

            this.electronProcess = new ElectronProcessPassive(ElectronNetRuntime.ElectronProcessId.Value);
            this.electronProcess.Ready += this.ElectronProcess_Ready;
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;

            this.electronProcess.Start();

            return Task.CompletedTask;
        }

        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
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
            this.socketBridge.Stop();
            return Task.CompletedTask;
        }
    }
}