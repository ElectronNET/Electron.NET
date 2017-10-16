using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;

namespace ElectronNET.WebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Bootstrap();
        }

        public async void Bootstrap()
        {
            var menuItems = new MenuItem[] {
                new MenuItem {
                    Label = "File",
                    Submenu = new MenuItem[] {
                        new MenuItem {
                            Label = "Exit",
                            Click = () =>
                            {
                                Electron.App.Exit();
                            }
                        }
                    }
                },
                new MenuItem
                {
                    Label = "About",
                    Click = async () => {
                        await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions("(c) 2017 Gregor Biswanger & Robert Muehsig") {
                            Title = "About us...",
                            Type = "info"
                        });
                    }
                }
            };

            Electron.Menu.SetApplicationMenu(menuItems);
            Electron.Tray.Show("/Assets/electron_32x32.png", menuItems);

            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Show = false
            });

            browserWindow.OnReadyToShow += async () =>
            {
                browserWindow.Show();

                await browserWindow.SetThumbarButtonsAsync(new ThumbarButton[] {
                    new ThumbarButton("/Assets/electron.ico")
                    {
                        Tooltip = "Hello World",
                        Click = async () => {
                            await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions("Hallo von Thumbar Button!"));
                        }
                    },
                    new ThumbarButton("/Assets/electron.ico")
                    {
                        Tooltip = "Hello World 2",
                        Click = async () => {
                            await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions("Hallo von Thumbar Button 2!"));
                        }
                    }
                });

                browserWindow.SetThumbnailToolTip("Electron.NET rocks!");
            };
        }
    }
}
