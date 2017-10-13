using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System;
using System.IO;

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

            App.IpcMain.On("GetPath", async (args) =>
            {
                string pathName = await App.GetPathAsync(PathName.pictures);
                //App.IpcMain.Send("GetPathComplete", pathName);

                var result = await App.GetPathAsync(PathName.exe);
                //var imagePath = Path.Combine(result, "Electron.png");
                App.IpcMain.Send("GetPathComplete", result);

                var image = await App.GetFileIconAsync(result);
            });

            return View();
        }
    }
}