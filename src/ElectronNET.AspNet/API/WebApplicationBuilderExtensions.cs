namespace ElectronNET.API
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Provides extension methods for <see cref="WebApplicationBuilder"/> to enable Electron.NET
    /// integration in ASP.NET Core applications (including Razor Pages) using the minimal hosting model.
    /// </summary>
    /// <remarks>
    /// Call this extension during host configuration (for example, in Program.cs) to wire up Electron
    /// with any command-line arguments and an optional application-ready callback.
    /// </remarks>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds Electron.NET support to the current ASP.NET Core application and registers an application-ready callback.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> to extend.</param>
        /// <param name="args">The command-line arguments passed to the process, forwarded to Electron.</param>
        /// <param name="onAppReadyCallback">
        /// An asynchronous callback invoked when the Electron app is ready. Use this to create windows or perform initialization.
        /// </param>
        /// <returns>
        /// The same <see cref="WebApplicationBuilder"/> instance to enable fluent configuration.
        /// </returns>
        /// <example>
        /// <code language="csharp">
        /// var builder = WebApplication.CreateBuilder(args)
        ///     .UseElectron(args, async () =>
        ///     {
        ///         // Create the main browser window or perform other startup tasks.
        ///     });
        ///
        /// var app = builder.Build();
        /// app.MapRazorPages();
        /// app.Run();
        /// </code>
        /// </example>
        public static WebApplicationBuilder UseElectron(this WebApplicationBuilder builder, string[] args, Func<Task> onAppReadyCallback)
        {
            return UseElectron(builder, options =>
            {
                options.Events = new()
                {
                    OnReady = onAppReadyCallback
                };
            });
        }

        public static WebApplicationBuilder UseElectron(this WebApplicationBuilder builder, Action<ElectronNetOptions> configure)
        {
            builder.WebHost.UseElectron(configure);

            return builder;
        }
    }
}