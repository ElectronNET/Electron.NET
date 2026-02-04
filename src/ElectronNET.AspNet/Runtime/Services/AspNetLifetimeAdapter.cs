namespace ElectronNET.AspNet.Runtime
{
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services;
    using Microsoft.Extensions.Hosting;

    internal class AspNetLifetimeAdapter : LifetimeServiceBase
    {
        private readonly IHostApplicationLifetime lifetimeService;

        public AspNetLifetimeAdapter(IHostApplicationLifetime lifetimeService)
        {
            this.lifetimeService = lifetimeService;

            this.lifetimeService.ApplicationStarted.Register(() => this.TransitionState(LifetimeState.Ready));
            this.lifetimeService.ApplicationStopping.Register(() => this.TransitionState(LifetimeState.Stopping));
            this.lifetimeService.ApplicationStopped.Register(() => this.TransitionState(LifetimeState.Stopped));
        }

        protected override Task StopCore()
        {
            this.lifetimeService.StopApplication();
            return Task.CompletedTask;
        }
    }
}