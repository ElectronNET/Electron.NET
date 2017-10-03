using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SayHello()
        {
            App.CreateNotification(new NotificationOptions {
                Title = "Hallo Robert",
                Body = "Nachricht von ASP.NET Core App"
            });

            return RedirectToAction("Index");
        }
    }
}