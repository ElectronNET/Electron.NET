namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.ElectronProcess;

    internal class RuntimeControllerAspNetElectronFirst : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private int? port;

        public RuntimeControllerAspNetElectronFirst(AspNetLifetimeAdapter aspNetLifetimeAdapter) : base(aspNetLifetimeAdapter)
        {
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

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

            this.CreateSocketBridge(this.port!.Value);

            this.electronProcess = new ElectronProcessPassive(ElectronNetRuntime.ElectronProcessId.Value);
            this.electronProcess.Stopped += this.ElectronProcess_Stopped;

            this.electronProcess.Start();

            Task.Run(() => this.TransitionState(LifetimeState.Started));

            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            this.electronProcess.Stop();
            return Task.CompletedTask;
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }
    }
}
