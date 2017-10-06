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
                }
            }

            if(IsElectronActive())
            {
                builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
            }

            return builder;
        }

        private static bool IsElectronActive()
        {
            return !string.IsNullOrEmpty(BridgeSettings.SocketPort);
        }
    }
}
