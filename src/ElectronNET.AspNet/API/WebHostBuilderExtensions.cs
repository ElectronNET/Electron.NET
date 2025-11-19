namespace ElectronNET.API
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ElectronNET;
    using ElectronNET.AspNet;
    using ElectronNET.AspNet.Runtime;
    using ElectronNET.Runtime;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides extension methods for <see cref="IWebHostBuilder"/> to enable Electron.NET
    /// integration in ASP.NET Core applications (including Razor Pages) using the WebHost-based hosting model.
    /// </summary>
    /// <remarks>
    /// Call this extension during web host configuration (for example, inside <c>ConfigureWebHostDefaults</c> in Program.cs)
    /// to wire up Electron with any command-line arguments and an optional application-ready callback.
    /// </remarks>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Adds Electron.NET support to the current ASP.NET Core web host and registers an application-ready callback.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to extend.</param>
        /// <param name="args">The command-line arguments passed to the process.</param>
        /// <param name="onAppReadyCallback">
        /// An asynchronous callback invoked when the Electron app is ready. Use this to create windows or perform initialization.
        /// </param>
        /// <returns>
        /// The same <see cref="IWebHostBuilder"/> instance to enable fluent configuration.
        /// </returns>
        /// <example>
        /// <code language="csharp">
        /// using Microsoft.AspNetCore.Hosting;
        /// using Microsoft.Extensions.Hosting;
        /// using ElectronNET.API;
        ///
        /// public class Program
        /// {
        ///     public static void Main(string[] args)
        ///     {
        ///         Host.CreateDefaultBuilder(args)
        ///             .ConfigureWebHostDefaults(webBuilder =>
        ///             {
        ///                 webBuilder.UseStartup&lt;Startup&gt;();
        ///                 webBuilder.UseElectron(args, async () =>
        ///                 {
        ///                     // Create the main browser window or perform other startup tasks.
        ///                 });
        ///             })
        ///             .Build()
        ///             .Run();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args, Func<Task> onAppReadyCallback)
        {
            if (onAppReadyCallback == null)
            {
                throw new ArgumentNullException(nameof(onAppReadyCallback));
            }

            // Backwards compatible overload – wraps the single callback into the new options model.
            return UseElectron(builder, options =>
            {
                options.Events.OnReady = onAppReadyCallback;
            });
        }

        /// <summary>
        /// Adds Electron.NET support to the current ASP.NET Core web host with granular lifecycle
        /// configuration. The provided <see cref="ElectronNetOptions"/> allows registration of callbacks
        /// for different phases of the Electron runtime.
        /// </summary>
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, Action<ElectronNetOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new ElectronNetOptions();
            configure(options);

            var host = ElectronHostEnvironment.InternalHost;
            host.ApplyOptions(options);

            var webPort = PortHelper.GetFreePort(host.AspNetWebPort ?? ElectronHostDefaults.DefaultWebPort);
            host.AspNetWebPort = webPort;

            // check for the content folder if its exists in base director otherwise no need to include
            // It was used before because we are publishing the project which copies everything to bin folder and contentroot wwwroot was folder there.
            // now we have implemented the live reload if app is run using /watch then we need to use the default project path.
            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\wwwroot"))
            {
                builder = builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .UseUrls("http://localhost:" + webPort);
            }
            else
            {
                builder = builder.UseUrls("http://localhost:" + webPort);
            }

            builder = builder.ConfigureServices(services =>
            {
                services.AddTransient<IStartupFilter, ServerReadyStartupFilter>();
                services.AddSingleton<AspNetLifetimeAdapter>();
                services.AddSingleton<IElectronHost>(host);

                switch (host.StartupMethod)
                {
                    case StartupMethod.PackagedElectronFirst:
                    case StartupMethod.UnpackedElectronFirst:
                        services.AddSingleton<IElectronNetRuntimeController, RuntimeControllerAspNetElectronFirst>();
                        break;
                    case StartupMethod.PackagedDotnetFirst:
                    case StartupMethod.UnpackedDotnetFirst:
                        services.AddSingleton<IElectronNetRuntimeController, RuntimeControllerAspNetDotnetFirst>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            return builder;
        }
    }
}