using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;

namespace ElectronNET.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        // WICHTIG! UseContentRoot auf Assembly Ordner Essentiell!
        // Ggf. kann man via Parameter den Content Root durchreichen?
        public static IWebHost BuildWebHost(string[] args)
        {
            // ToDo: Maybe add a "electronized" args check here?
            // this is the electron case!
            if (args.Length > 0)
            {
                Console.WriteLine("Test Switch for Electron detection: " + args[0]);
                return WebHost.CreateDefaultBuilder(args)
                    .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .UseStartup<Startup>()
                    .Build();
            }
            else
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .Build();
            }
                
            // this didn't work... its too late, idk... 
            //var builder = WebHost.CreateDefaultBuilder(args);

            //if (args.Length > 0)
            //{
            //    // ToDo: Maybe add a "electronized" args check here?
            //    Console.WriteLine("Test Switch for Electron detection: " + args[0]);
            //    builder = builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
            //}

            //builder = builder.UseStartup<Startup>();

            //return builder.Build();
        }
    }
}
