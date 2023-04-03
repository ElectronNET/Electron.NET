using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Use a Electron support for this .NET Core Project.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IWebHostBuilder UseElectron(this IWebHostBuilder builder, string[] args)
        {
            foreach (string argument in args)
            {
                if (argument.ToUpper().Contains("ELECTRONPORT"))
                {
                    BridgeSettings.SocketPort = argument.ToUpper().Replace("/ELECTRONPORT=", "");
                    Console.WriteLine("Use Electron Port: " + BridgeSettings.SocketPort);
                }
                else if (argument.ToUpper().Contains("ELECTRONWEBPORT"))
                {
                    BridgeSettings.WebPort = argument.ToUpper().Replace("/ELECTRONWEBPORT=", "");
                }
            }

            if (HybridSupport.IsElectronActive)
            {
                builder.ConfigureServices(services =>
                {
                    services.AddHostedService<LifetimeServiceHost>();
                });

                // check for the content folder if its exists in base director otherwise no need to include
                // It was used before because we are publishing the project which copies everything to bin folder and contentroot wwwroot was folder there.
                // now we have implemented the live reload if app is run using /watch then we need to use the default project path.
                if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\wwwroot"))
                {
                    builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                        .UseUrls("http://localhost:" + BridgeSettings.WebPort);
                }
                else
                {
                    builder.UseUrls("http://localhost:" + BridgeSettings.WebPort);
                }
            }

            return builder;
        }
    }
}