using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

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
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                .UseStartup<Startup>()
                .Build();
    }
}
