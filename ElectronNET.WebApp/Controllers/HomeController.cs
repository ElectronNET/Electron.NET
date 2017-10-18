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
            Electron.IpcMain.On("SayHello", async (args) =>
            {
                Electron.Notification.Show(new NotificationOptions("Hallo Robert", "Nachricht von ASP.NET Core App"));
                Electron.IpcMain.Send(Electron.WindowManager.BrowserWindows.First(), "Goodbye", "Elephant!");

                var currentBrowserWindow = Electron.WindowManager.BrowserWindows.First();
                var openDialogOptions = new OpenDialogOptions
                {
                    Title = "Wuhuuu",
                    ButtonLabel = "Mhh Okay",
                    DefaultPath = await Electron.App.GetPathAsync(PathName.pictures),
                    Message = "Hello World",
                    Properties = new OpenDialogProperty[] { OpenDialogProperty.openDirectory }
                };
                var filePaths = await Electron.Dialog.ShowOpenDialogAsync(currentBrowserWindow, openDialogOptions);
            });

            Electron.IpcMain.On("GetPath", async (args) =>
            {
                Electron.Notification.Show(new NotificationOptions("test", "test2"));

                var currentBrowserWindow = Electron.WindowManager.BrowserWindows.First();

                string pathName = await Electron.App.GetPathAsync(PathName.pictures);
                Electron.IpcMain.Send(currentBrowserWindow, "GetPathComplete", pathName);

                currentBrowserWindow.Minimize();
                await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions {
                    Title = "My second Window",
                    AutoHideMenuBar = true
                },"http://www.google.de");
            });
            

            return View();
        }
    }
}