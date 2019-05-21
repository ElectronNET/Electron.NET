using System.Linq;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;

namespace ElectronNET.WebApp.Controllers
{
    public class UpdateController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("auto-update", async (args) =>
                {
                    // Electron.NET CLI Command for deploy:
                    // electronize build /target win /electron-params --publish=always
          
                    var currentVersion = await Electron.App.GetVersionAsync();
                    var updateCheckResult = await Electron.AutoUpdater.CheckForUpdatesAndNotifyAsync();
                    var availableVersion = updateCheckResult.UpdateInfo.Version;
                    string information = $"Current version: {currentVersion} - available version: {availableVersion}";

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "auto-update-reply", information);
                });
            }

            return View();
        }
    }
}