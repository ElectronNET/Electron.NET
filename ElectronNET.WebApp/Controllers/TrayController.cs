using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class TrayController : Controller
    {
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

                        Electron.Tray.Show("/Assets/electron_32x32.png", menu);
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