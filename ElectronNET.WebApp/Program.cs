using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ElectronNET.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IWebHostBuilder builder;

#if DEBUG
            var webPort = Electron.Experimental.FreeTcpPort();
            
            await Electron.Experimental.StartElectronForDevelopment(webPort);

            builder = CreateWebHostBuilder(args);
            // check for the content folder if its exists in base director otherwise no need to include
            // It was used before because we are publishing the project which copies everything to bin folder and contentroot wwwroot was folder there.
            // now we have implemented the live reload if app is run using /watch then we need to use the default project path.
            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\wwwroot"))
            {
                builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
            }
            
            builder.UseUrls("http://localhost:" + webPort);
#else
            builder = CreateWebHostBuilder(args);
            Debugger.Launch();
            Electron.ReadAuth();
            builder.UseElectron(args);
#endif

            await builder.Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); })
                .UseStartup<Startup>();
        }
    }
}
