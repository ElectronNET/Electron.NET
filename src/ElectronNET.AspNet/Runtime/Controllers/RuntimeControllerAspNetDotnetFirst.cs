namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.ElectronProcess;

    internal class RuntimeControllerAspNetDotnetFirst : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private int? port;

        public RuntimeControllerAspNetDotnetFirst(AspNetLifetimeAdapter aspNetLifetimeAdapter) : base(aspNetLifetimeAdapter)
        {
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

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

        protected override Task StopCore()
        {
            this.electronProcess.Stop();
            return Task.CompletedTask;
        }

        private void ElectronProcess_Ready(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Started);
            this.CreateSocketBridge(this.port!.Value);
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }
    }
}
