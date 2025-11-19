using ElectronNET.API;
using ElectronNET.Runtime;

namespace ElectronNET.WebApp
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.API.Entities;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var runtimeController = ElectronHostEnvironment.Current.RuntimeController;

            try
            {
                await runtimeController.Start();

                await runtimeController.WaitReadyTask;

                await ElectronBootstrap();

                await runtimeController.WaitStoppedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await runtimeController.Stop().ConfigureAwait(false);

                await runtimeController.WaitStoppedTask.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            }
        }

        public static async Task ElectronBootstrap()
        {
            //AddDevelopmentTests();

            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false,
            }, "https://github.com/ElectronNET/Electron.NET");

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
        }
    }
}
