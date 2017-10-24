using Microsoft.AspNetCore.Mvc;

namespace ElectronNET.WebApp.Controllers
{
    public class DesktopCapturerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}