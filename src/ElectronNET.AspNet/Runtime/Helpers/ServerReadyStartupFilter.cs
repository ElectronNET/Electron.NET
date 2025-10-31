namespace ElectronNET.AspNet
{
    using System;
    using ElectronNET.AspNet.Runtime;
    using ElectronNET.Runtime;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class ServerReadyStartupFilter : IStartupFilter
    {
        /// <summary>
        /// Extends the provided <paramref name="next" /> and returns an <see cref="T:System.Action" /> of the same type.
        /// </summary>
        /// <param name="next">The Configure method to extend.</param>
        /// <returns>A modified <see cref="T:System.Action" />.</returns>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                _ = app.ApplicationServices.GetService<AspNetLifetimeAdapter>();
                var runtimeController = app.ApplicationServices.GetService<IElectronNetRuntimeController>();

                runtimeController.Start();

                next(app);
            };
        }
    }
}
