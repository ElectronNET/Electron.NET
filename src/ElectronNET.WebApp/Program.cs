using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectronNET.WebApp
{
    using System.Threading.Tasks;
    using ElectronNET.API.Entities;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); })
                .UseElectron(args, ElectronBootstrap)
                .UseStartup<Startup>();
        }

        public static async Task ElectronBootstrap()
        {
            //AddDevelopmentTests();

            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
        }

        private static void AddDevelopmentTests()
        {
            // NOTE: on mac you will need to allow the app to post notifications when asked.

            _ = Electron.App.On("activate", (obj) =>
            {
                // obj should be a boolean that represents where there are active windows or not.
                var hasWindows = (bool)obj;

                Electron.Notification.Show(
                    new NotificationOptions("Activate", $"activate event has been captured. Active windows = {hasWindows}")
                    {
                        Silent = false,
                    });
            });

            Electron.Dock.SetMenu(new[]
            {
                new MenuItem
                {
                    Type = MenuType.normal,
                    Label = "MenuItem",
                    Click = () =>
                    {
                        Electron.Notification.Show(new NotificationOptions(
                            "Dock MenuItem Click",
                            "A menu item added to the Dock was selected;"));
                    },
                },
                new MenuItem
                {
                    Type = MenuType.submenu,
                    Label = "SubMenu",
                    Submenu = new[]
                    {
                        new MenuItem
                        {
                            Type = MenuType.normal,
                            Label = "Sub MenuItem",
                            Click = () =>
                            {
                                Electron.Notification.Show(new NotificationOptions(
                                    "Dock Sub MenuItem Click",
                                    "A menu item added to the Dock was selected;"));
                            },
                        },
                    }
                }
            });
        }
    }
}
