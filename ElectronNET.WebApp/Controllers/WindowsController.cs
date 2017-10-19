using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;

namespace ElectronNET.WebApp.Controllers
{
    public class WindowsController : Controller
    {
        public IActionResult Index()
        {
            Electron.IpcMain.On("new-window", async (args) => {

                await Electron.WindowManager.CreateWindowAsync();

            });

            return View();
        }
    }
}