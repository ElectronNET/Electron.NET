namespace ElectronNET.AspNet.Runtime
{
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services;
    using Microsoft.Extensions.Hosting;

    internal  class AspNetLifetimeAdapter : LifetimeServiceBase
    {
        private readonly IHostApplicationLifetime lifetimeSercice;

        public AspNetLifetimeAdapter(IHostApplicationLifetime lifetimeSercice)
        {
            this.lifetimeSercice = lifetimeSercice;

            this.lifetimeSercice.ApplicationStarted.Register(() => this.TransitionState(LifetimeState.Ready));
            this.lifetimeSercice.ApplicationStopping.Register(() => this.TransitionState(LifetimeState.Stopping));
            this.lifetimeSercice.ApplicationStopped.Register(() => this.TransitionState(LifetimeState.Stopped));
        }

        protected override Task StopCore()
        {
            this.lifetimeSercice.StopApplication();
            return Task.CompletedTask;
        }
    }
}
