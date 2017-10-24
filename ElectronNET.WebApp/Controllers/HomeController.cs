using Microsoft.AspNetCore.Mvc;

namespace ElectronNET.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}