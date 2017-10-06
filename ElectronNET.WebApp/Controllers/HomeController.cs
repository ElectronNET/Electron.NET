using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            App.IpcMain.On("SayHello", (args) => {
                App.CreateNotification(new NotificationOptions
                {
                    Title = "Hallo Robert",
                    Body = "Nachricht von ASP.NET Core App"
                });

                App.IpcMain.Send("Goodbye", "Elephant!");
            });

            return View();
        }
    }
}