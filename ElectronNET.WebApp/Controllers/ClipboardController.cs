using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using System.Linq;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class ClipboardController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("copy-to", (text) =>
                {
                    Electron.Clipboard.WriteText(text.ToString());
                });

                Electron.IpcMain.On("paste-to", async (text) =>
                {
                    Electron.Clipboard.WriteText(text.ToString());
                    string pasteText = await Electron.Clipboard.ReadTextAsync();

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "paste-from", pasteText);
                });

                Electron.IpcMain.On("copy-image-to",  (test) =>
                {
                    var nativeImage = NativeImage.CreateFromDataURL(test.ToString());
                    Electron.Clipboard.WriteImage(nativeImage);
                });
            }

            return View();
        }
    }
}