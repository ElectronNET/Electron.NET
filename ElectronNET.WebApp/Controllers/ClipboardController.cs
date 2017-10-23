using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using System.Linq;

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
            }

            return View();
        }
    }
}