using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Electron.IpcMain.On("SayHello", (args) => {
                Electron.App.CreateNotification(new NotificationOptions
                {
                    Title = "Hallo Robert",
                    Body = "Nachricht von ASP.NET Core App"
                });

                Electron.IpcMain.Send("Goodbye", "Elephant!");
            });

            Electron.IpcMain.On("GetPath", async (args) =>
            {
                string pathName = await Electron.App.GetPathAsync(PathName.pictures);
                //App.IpcMain.Send("GetPathComplete", pathName);

                var result = await Electron.App.GetPathAsync(PathName.exe);
                //var imagePath = Path.Combine(result, "Electron.png");
                Electron.IpcMain.Send("GetPathComplete", result);

              
                //var image = await App.GetFileIconAsync(result);
                
            });

            return View();
        }
    }
}