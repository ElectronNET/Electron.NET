using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ElectronNET.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "..", "ElectronNET.WebApp"))
                .UseStartup<Startup>()
                .Build();
    }
}
