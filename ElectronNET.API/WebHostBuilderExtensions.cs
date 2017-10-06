using Microsoft.AspNetCore.Hosting;
using System;

namespace ElectronNET.API
{
    public static class WebHostBuilderExtensions
    {
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

            if(IsElectronActive())
            {
                builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .UseUrls("http://0.0.0.0:" + BridgeSettings.WebPort);
            }

            return builder;
        }

        private static bool IsElectronActive()
        {
            return !string.IsNullOrEmpty(BridgeSettings.SocketPort);
        }
    }
}
