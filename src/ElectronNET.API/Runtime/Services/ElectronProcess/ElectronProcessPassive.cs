namespace ElectronNET.Runtime.Services.ElectronProcess
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;

    /// <summary>
    /// Launches and manages the Electron app process.
    /// </summary>
    [Localizable(false)]
    internal class ElectronProcessPassive : ElectronProcessBase
    {
        private readonly int pid;
        private Process process;

        /// <summary>Initializes a new instance of the <see cref="ElectronProcessPassive"/> class.</summary>
        /// <param name="pid"></param>
        public ElectronProcessPassive(int pid)
        {
            this.pid = pid;
        }

        protected override Task StartCore()
        {
            this.process = Process.GetProcessById(this.pid);

            if (this.process == null)
            {
                throw new ArgumentException($"Unable to find process with ID {this.pid}");
            }

            this.process.Exited += this.Process_Exited1;

            Task.Run(() => this.TransitionState(LifetimeState.Ready));

            return Task.CompletedTask;
        }

        private void Process_Exited1(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Stopped);
        }

        protected override Task StopCore()
        {
            // Not sure about this:
            ////this.process.Kill(true);
            return Task.CompletedTask;
        }
    }
}