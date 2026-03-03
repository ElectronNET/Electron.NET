namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting.Server;
    using ElectronNET.AspNet.Services;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services.ElectronProcess;

    internal class RuntimeControllerAspNetElectronFirst : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;

        public RuntimeControllerAspNetElectronFirst(IServer server, AspNetLifetimeAdapter aspNetLifetimeAdapter, IElectronAuthenticationService authenticationService = null) : base(server, aspNetLifetimeAdapter, authenticationService)
        {
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

        protected override Task StartCore()
        {
            var port = ElectronNetRuntime.ElectronSocketPort.Value;
            var token = ElectronNetRuntime.ElectronAuthToken;

            if (!ElectronNetRuntime.ElectronProcessId.HasValue)
            {
                throw new Exception("No electronPID has been specified by Electron!");
            }

            this.CreateSocketBridge(port, token);

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
