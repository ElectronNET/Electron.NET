using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ElectronNET.WebApp.Controllers
{
    public class ShellController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("open-file-manager", async (args) =>
                {
                    string path = await Electron.App.GetPathAsync(PathName.home);
                    await Electron.Shell.ShowItemInFolderAsync(path);

                });

                Electron.IpcMain.On("open-ex-links", async (args) =>
                {
                    await Electron.Shell.OpenExternalAsync("https://github.com/ElectronNET");
                });
            }

            return View();
        }
    }
}