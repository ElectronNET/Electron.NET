using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class AppSysInformationController : Controller
    {
        public IActionResult Index()
        {
            if(HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.OnWithId("app-info", async (id, args) =>
                {
                    string appPath = await Electron.App.GetAppPathAsync();

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "got-app-path", appPath);
                });

                Electron.IpcMain.OnWithId("sys-info", async (id, args) =>
                {
                    string homePath = await Electron.App.GetPathAsync(PathName.Home);

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "got-sys-info", homePath);
                });

                Electron.IpcMain.OnWithId("screen-info", async (id, args) =>
                {
                    var display = await Electron.Screen.GetPrimaryDisplayAsync();

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "got-screen-info", display.Size);
                });
            }

            return View();
        }
    }
}