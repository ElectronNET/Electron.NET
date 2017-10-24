using Microsoft.AspNetCore.Hosting;
using System;

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
                } else if(argument.ToUpper().Contains("ELECTRONWEBPORT"))
                {
                    BridgeSettings.WebPort = argument.ToUpper().Replace("/ELECTRONWEBPORT=", "");
                }
            }

            if(HybridSupport.IsElectronActive)
            {
                builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .UseUrls("http://0.0.0.0:" + BridgeSettings.WebPort);
            }

            return builder;
        }
    }
}
