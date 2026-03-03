namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting.Server;
    using ElectronNET.AspNet.Services;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using System.Security.Principal;

    internal class RuntimeControllerAspNetDotnetFirst : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;

        public RuntimeControllerAspNetDotnetFirst(IServer server, AspNetLifetimeAdapter aspNetLifetimeAdapter, IElectronAuthenticationService authenticationService = null) : base(server, aspNetLifetimeAdapter, authenticationService)
        {
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

        protected override Task StartCore()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var electronBinaryName = ElectronNetRuntime.ElectronExecutable;
            var port = ElectronNetRuntime.ElectronSocketPort ?? 0;
            var args = Environment.CommandLine;

            this.electronProcess = new ElectronProcessActive(isUnPacked, electronBinaryName, args, port);
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
            var port = ElectronNetRuntime.ElectronSocketPort.Value;
            var token = ElectronNetRuntime.ElectronAuthToken;
            this.TransitionState(LifetimeState.Started);
            this.CreateSocketBridge(port, token);
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }
    }
}
