using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ElectronNET.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

#if DEBUG
            //Uncomment this line to automatically attach the Debugger on launch. This should only be used in development
            //Debugger.Launch();
#endif

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); })
                .UseElectron(args)
                .UseStartup<Startup>();
        }
    }
}
