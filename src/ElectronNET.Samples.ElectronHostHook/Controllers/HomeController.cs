using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronNET.Samples.ElectronHostHook.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string message = "Electron not active";
            if (HybridSupport.IsElectronActive)
            {
                // Call the HostHook defined in ElectronHostHook/index.ts
                var result = await Electron.HostHook.CallAsync<string>("ping", "Hello from C#");
                message = $"Sent 'Hello from C#', Received: '{result}'";
            }

            return View("Index", message);
        }
    }
}
