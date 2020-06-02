using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ElectronNET.API
{
    /// <summary>
    /// Base class that reports if ASP.NET Core has fully started.
    /// </summary>
    internal class LifetimeServiceHost : IHostedService
    {
        public LifetimeServiceHost(IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                App.Instance.IsReady = true;

                Console.WriteLine("ASP.NET Core host has fully started.");
            });
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}