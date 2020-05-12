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
                    Console.WriteLine("Screen Locked detected from C #");
                };

                Electron.PowerMonitor.OnUnLockScreen += () =>
                {
                    Console.WriteLine("Screen unlocked detected from C # ");
                };
            }
                return View();
        }
    }
}