using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class WindowsController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                string viewPath = $"http://localhost:{BridgeSettings.WebPort}/windows/demowindow";

                Electron.IpcMain.On("new-window", async (args) =>
                {

                    await Electron.WindowManager.CreateWindowAsync(viewPath);

                });

                Electron.IpcMain.On("manage-window", async (args) =>
                {

                    var browserWindow = await Electron.WindowManager.CreateWindowAsync(viewPath);
                    browserWindow.OnMove += UpdateReply;
                    browserWindow.OnResize += UpdateReply;
                });

                Electron.IpcMain.On("listen-to-window", async (args) =>
                {
                    var mainBrowserWindow = Electron.WindowManager.BrowserWindows.First();

                    var browserWindow = await Electron.WindowManager.CreateWindowAsync(viewPath);
                    browserWindow.OnFocus += () => Electron.IpcMain.Send(mainBrowserWindow, "listen-to-window-focus");
                    browserWindow.OnBlur += () => Electron.IpcMain.Send(mainBrowserWindow, "listen-to-window-blur");

                    Electron.IpcMain.On("listen-to-window-set-focus", (x) => browserWindow.Focus());
                });

                Electron.IpcMain.On("frameless-window", async (args) =>
                {
                    var options = new BrowserWindowOptions
                    {
                        Frame = false
                    };
                    await Electron.WindowManager.CreateWindowAsync(options, viewPath);
                });
            }

            return View();
        }

        private async void UpdateReply()
        {
            var browserWindow = Electron.WindowManager.BrowserWindows.Last();
            var size = await browserWindow.GetSizeAsync();
            var position = await browserWindow.GetPositionAsync();
            string message = $"Size: {size[0]},{size[1]} Position: {position[0]},{position[1]}";

            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            Electron.IpcMain.Send(mainWindow, "manage-window-reply", message);
        }

        public IActionResult DemoWindow()
        {
            return View();
        }
    }
}