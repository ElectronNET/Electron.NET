using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNET.API.Hubs;
using ElectronNET.API.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.WebApp
{
    public class Program
    {
        public static bool WebServerReady = false;

        [STAThread]
        public static async Task Main(string[] args)
        {
            await WebApplication.ProcessInit(args);
        }

        public static class WebApplication
        {
            public static async Task ProcessInit(string[] args)
            {
                await Startup.RunWebHost(args);
            }
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); })
                .UseStartup<Startup>();
        }
    }
}
