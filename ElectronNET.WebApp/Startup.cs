using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNET.API.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ElectronNET.API.Models;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ElectronNET.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
            // ToDo: Replace newtonsoft woth system.text.json
            /*.AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });*/
            .AddNewtonsoftJsonProtocol(options => {
                options.PayloadSerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Electron.ServiceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<HubElectron>("/electron");
            });

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }

            Electron.SignalrElectron = Electron.ServiceScope.ServiceProvider.GetService<IHubContext<HubElectron>>();

            Program.WebServerReady = true;
        }


        public static async Task RunWebHost(string[] args, bool runBlocking = true)
        {
            IHost host = new HostBuilder()
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); });
                    webBuilder.ConfigureKestrel(option =>
                    {
                        option.Listen(IPAddress.Any, 5000);
                        //option.AllowSynchronousIO = true;
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .Build();
            if (runBlocking)
            {
                host.Run();
            }
            else
            {
                host.Start();
            }

        }

        public async void ElectronBootstrap()
        {
            // Kill all existing electron processes
            foreach (var processInterface in Process.GetProcessesByName("electron"))
            {
                try
                {
                    processInterface.Kill();
                }
                catch
                {
                    // ignored
                }
            }

            string EntryAssembly = Assembly.GetEntryAssembly()?.Location;
            string EntryAssemblyPath = Path.GetDirectoryName(EntryAssembly)?.Replace("\\", "/");
            string parentPath = Directory.GetParent(EntryAssemblyPath)?.FullName;
            string parentParentPath = Directory.GetParent(parentPath)?.FullName;
            string parentParentParentPath = Directory.GetParent(parentParentPath)?.FullName;
            string parentParentParentParentPath = Directory.GetParent(parentParentParentPath)?.FullName;

            if (File.Exists(parentParentParentParentPath + "/ElectronNET.Host/node_modules/.bin/electron.cmd"))
            {
                Process process = new Process();
                process.StartInfo.FileName = parentParentParentParentPath + "/ElectronNET.Host/node_modules/.bin/electron.cmd";
                process.StartInfo.Arguments = parentParentParentParentPath + "/ElectronNET.Host/main.js";
                process.StartInfo.UseShellExecute = true; // Open own window
                process.Start();
            }

            while (!Electron.ElectronConnected)
            {
                await Task.Delay(500);
            }

            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Electron.NET API Demos");

            if (Electron.Tray.MenuItems.Count == 0)
            {
                var menu = new MenuItem
                {
                    Label = "Remove",
                    Click = () => Electron.Tray.Destroy()
                };

                Electron.Tray.Show(Path.Combine(WebHostEnvironment.ContentRootPath, "Assets/electron_32x32.png"), menu);
                Electron.Tray.SetToolTip("Electron Demo in the tray.");
            }

        }

    }
}
