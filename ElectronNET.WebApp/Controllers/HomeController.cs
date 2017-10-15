using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Linq;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Electron.IpcMain.On("SayHello", (args) => {
                Electron.Notification.Show(new NotificationOptions("Hallo Robert","Nachricht von ASP.NET Core App"));

                Electron.IpcMain.Send("Goodbye", "Elephant!");
            });

            Electron.IpcMain.On("GetPath", async (args) =>
            {
                string pathName = await Electron.App.GetPathAsync(PathName.pictures);
                Electron.IpcMain.Send("GetPathComplete", pathName);

                Electron.WindowManager.BrowserWindows.First().Minimize();
                await Electron.WindowManager.CreateWindowAsync("http://www.google.de");
            });
            

            return View();
        }
    }
}