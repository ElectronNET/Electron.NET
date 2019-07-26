using System.IO;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;

namespace ElectronNET.WebApp.Controllers
{
    public class TrayController : Controller
    {
        private readonly IHostingEnvironment _env;

        public TrayController(IHostingEnvironment env)
        {
            _env = env;
        }


        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("put-in-tray", (args) =>
                {

                    if (Electron.Tray.MenuItems.Count == 0)
                    {
                        var menu = new MenuItem
                        {
                            Label = "Remove",
                            Click = () => Electron.Tray.Destroy()
                        };

                        Electron.Tray.Show(Path.Combine(_env.ContentRootPath, "Assets/electron_32x32.png"), menu);
                        Electron.Tray.SetToolTip("Electron Demo in the tray.");
                    }
                    else
                    {
                        Electron.Tray.Destroy();
                    }

                });
            }

            return View();
        }
    }
}