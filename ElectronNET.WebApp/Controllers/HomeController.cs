using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.PowerMonitor.OnLockScreen += () =>
                {
                    Console.WriteLine("Screeen Bloqueado desde C#");
                };

                Electron.PowerMonitor.OnUnLockScreen += () =>
                {
                    Console.WriteLine("Desbloquedo desde C#");
                };
            }
                return View();
        }
    }
}