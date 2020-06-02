using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
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
                    Console.WriteLine("Screen Locked detected from C#");
                };

                Electron.PowerMonitor.OnUnLockScreen += () =>
                {
                    Console.WriteLine("Screen unlocked detected from C# ");
                };

                Electron.PowerMonitor.OnSuspend += () =>
                {
                    Console.WriteLine("The system is going to sleep");
                };

                Electron.PowerMonitor.OnResume += () =>
                {
                    Console.WriteLine("The system is resuming");
                };

                Electron.PowerMonitor.OnAC += () =>
                {
                    Console.WriteLine("The system changes to AC power");
                };

                Electron.PowerMonitor.OnBattery += () =>
                {
                    Console.WriteLine("The system is about to change to battery power");
                };

            }
            
            return View();
        }
    }
}