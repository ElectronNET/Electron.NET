namespace ElectronNET.AspNet.Runtime
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.AspNet.Services;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.ElectronProcess;

    internal class RuntimeControllerAspNetDotnetFirst : RuntimeControllerAspNetBase
    {
        private ElectronProcessBase electronProcess;
        private readonly string authorization;

        public RuntimeControllerAspNetDotnetFirst(AspNetLifetimeAdapter aspNetLifetimeAdapter, IElectronAuthenticationService authenticationService = null) : base(aspNetLifetimeAdapter)
        {
            this.authorization = Guid.NewGuid().ToString("N"); // 32 hex chars, no hyphens

            // Only if somebody registered an IElectronAuthenticationService service - otherwise we do not care
            authenticationService?.SetExpectedToken(this.authorization);
        }

        internal override ElectronProcessBase ElectronProcess => this.electronProcess;

        protected override Task StartCore()
        {
            var isUnPacked = ElectronNetRuntime.StartupMethod.IsUnpackaged();
            var electronBinaryName = ElectronNetRuntime.ElectronExecutable;
            var authToken = this.authorization;
            var port = ElectronNetRuntime.ElectronSocketPort ?? 0;
            var args = $"{Environment.CommandLine} --authtoken={authToken}";

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
            this.TransitionState(LifetimeState.Started);
            this.CreateSocketBridge(port, this.authorization);
        }

        private void ElectronProcess_Stopped(object sender, EventArgs e)
        {
            this.HandleStopped();
        }
    }
}
