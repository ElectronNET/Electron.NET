using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.WebApp.Controllers
{
    public class CrashHangController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("process-crash", async (args) =>
                {
                    string viewPath = $"http://localhost:{BridgeSettings.WebPort}/crashhang/processcrash";

                    var browserWindow = await Electron.WindowManager.CreateWindowAsync(viewPath);
                    browserWindow.WebContents.OnCrashed += async (killed) =>
                    {
                        var options = new MessageBoxOptions("This process has crashed.")
                        {
                            Type = MessageBoxType.info,
                            Title = "Renderer Process Crashed",
                            Buttons = new string[] { "Reload", "Close" }
                        };
                        var result = await Electron.Dialog.ShowMessageBoxAsync(options);

                        if (result.Response == 0)
                        {
                            browserWindow.Reload();
                        }
                        else
                        {
                            browserWindow.Close();
                        }
                    };
                });

                Electron.IpcMain.On("process-hang", async (args) =>
                {
                    string viewPath = $"http://localhost:{BridgeSettings.WebPort}/crashhang/processhang";

                    var browserWindow = await Electron.WindowManager.CreateWindowAsync(viewPath);
                    browserWindow.OnUnresponsive += async () =>
                    {
                        var options = new MessageBoxOptions("This process is hanging.")
                        {
                            Type = MessageBoxType.info,
                            Title = "Renderer Process Hanging",
                            Buttons = new string[] { "Reload", "Close" }
                        };
                        var result = await Electron.Dialog.ShowMessageBoxAsync(options);

                        if (result.Response == 0)
                        {
                            browserWindow.Reload();
                        }
                        else
                        {
                            browserWindow.Close();
                        }
                    };
                });
            }

            return View();
        }

        public IActionResult ProcessCrash()
        {
            return View();
        }

        public IActionResult ProcessHang()
        {
            return View();
        }
    }
}