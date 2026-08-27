namespace ElectronNET.API
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using ElectronNET.AspNet;
    using ElectronNET.AspNet.Runtime;
    using ElectronNET.Runtime;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;
    using ElectronNET.Runtime.Services.SocketBridge;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

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
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAppReadyCallbackResolver>(_ => new AppReadyCallbackResolver(onAppReadyCallback));
            });

            return UseElectronCore(builder, args);
        }

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
        ///                 webBuilder.UseElectron(args, async (processArgs) =>
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
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args, Func<string[], Task> onAppReadyCallback)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAppReadyCallbackResolver>(_ => new AppReadyCallbackResolver(args, onAppReadyCallback));
            });

            return UseElectronCore(builder, args);
        }

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
        ///                 webBuilder.UseElectron(args, async (serviceProvider) =>
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
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args, Func<IServiceProvider, Task> onAppReadyCallback)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAppReadyCallbackResolver>(provider => new AppReadyCallbackResolver(provider, onAppReadyCallback));
            });

            return UseElectronCore(builder, args);
        }

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
        ///                 webBuilder.UseElectron(args, async (serviceProvider, processArgs) =>
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
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args, Func<IServiceProvider, string[], Task> onAppReadyCallback)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAppReadyCallbackResolver>(provider => new AppReadyCallbackResolver(provider, args, onAppReadyCallback));
            });

            return UseElectronCore(builder, args);
        }

        private static IWebHostBuilder UseElectronCore(IWebHostBuilder builder, string[] args)
        {
            // no matter how this is set - let's unset to prevent Electron not starting as expected
            // e.g., VS Code sets this env variable, but this will cause `require("electron")` to not
            // work as expected, see issue #952
            Environment.SetEnvironmentVariable("ELECTRON_RUN_AS_NODE", null);

            var webPort = ElectronNetRuntime.AspNetWebPort ?? 0;

            // In packaged mode, static content is deployed alongside the app binaries, so we must
            // point content root to the process base directory. In unpackaged/watch scenarios we
            // keep the default project content root to preserve live reload behavior.
            var isPackagedStartup = ElectronNetRuntime.StartupMethod == StartupMethod.PackagedElectronFirst ||
                ElectronNetRuntime.StartupMethod == StartupMethod.PackagedDotnetFirst;

            // For port 0 (dynamic port assignment), Kestrel requires binding to specific IP (127.0.0.1) not localhost
            var host = webPort == 0 ? "127.0.0.1" : "localhost";

            if (isPackagedStartup)
            {
                builder = builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .UseUrls($"http://{host}:{webPort}");
            }
            else
            {
                builder = builder.UseUrls($"http://{host}:{webPort}");
            }

            builder = builder.ConfigureServices((context, services) =>
            {
                services.AddTransient<IStartupFilter, ServerReadyStartupFilter>();
                services.AddSingleton<AspNetLifetimeAdapter>();
                services.AddSingleton<ISocketBridgeServiceFactory>(sp =>
                    new SocketBridgeServiceFactory(sp.GetService<IOptions<ElectronSocketIOOptions>>()?.Value.ConfigureServices));

                switch (ElectronNetRuntime.StartupMethod)
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