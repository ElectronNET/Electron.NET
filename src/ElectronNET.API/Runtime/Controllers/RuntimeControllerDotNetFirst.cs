namespace ElectronNET.Runtime.Controllers
{
    using ElectronNET.API;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using ElectronNET.Runtime.Services.SocketBridge;
    using System;
    using System.Threading.Tasks;

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
            var host = ElectronHostEnvironment.InternalHost;
            var isUnPacked = host.StartupMethod.IsUnpackaged();
            var electronBinaryName = host.ElectronExecutable;
            var args = string.Format("{0} {1}", host.ElectronExtraArguments, Environment.CommandLine).Trim();
            this.port = host.ElectronSocketPort;

            if (!this.port.HasValue)
            {
                this.port = PortHelper.GetFreePort(ElectronHostDefaults.DefaultSocketPort);
                host.ElectronSocketPort = this.port;
            }

            Console.Error.WriteLine("[StartCore]: isUnPacked: {0}", isUnPacked);
            Console.Error.WriteLine("[StartCore]: electronBinaryName: {0}", electronBinaryName);
            Console.Error.WriteLine("[StartCore]: args: {0}", args);

            this.electronProcess = new ElectronProcessActive(isUnPacked, electronBinaryName, args, this.port.Value);
            this.electronProcess.Ready += this.ElectronProcess_Ready;
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;

            Console.Error.WriteLine("[StartCore]: Before Start");
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
            if (this.socketBridge != null && this.socketBridge.State != LifetimeState.Stopped)
            {
                this.socketBridge.Stop();
            }
            else if (this.electronProcess != null && this.electronProcess.State != LifetimeState.Stopped)
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