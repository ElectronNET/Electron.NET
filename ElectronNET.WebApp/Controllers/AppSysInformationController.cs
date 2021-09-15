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
                Electron.IpcMain.OnWithId("app-info", async (info) =>
                {
                    string appPath = await Electron.App.GetAppPathAsync();

                    if (Electron.WindowManager.TryGetBrowserWindows(info.browserId, out var window))
                    {
                        Electron.IpcMain.Send(window, "got-app-path", appPath);
                    }
                });

                Electron.IpcMain.OnWithId("sys-info", async (info) =>
                {
                    string homePath = await Electron.App.GetPathAsync(PathName.Home);

                    if (Electron.WindowManager.TryGetBrowserWindows(info.browserId, out var window))
                    {
                        Electron.IpcMain.Send(window, "got-sys-info", homePath);
                    }
                });

                Electron.IpcMain.OnWithId("screen-info", async (info) =>
                {
                    var display = await Electron.Screen.GetPrimaryDisplayAsync();

                    if (Electron.WindowManager.TryGetBrowserWindows(info.browserId, out var window))
                    {
                        Electron.IpcMain.Send(window, "got-screen-info", display.Size);
                    }
                });
            }

            return View();
        }
    }
}