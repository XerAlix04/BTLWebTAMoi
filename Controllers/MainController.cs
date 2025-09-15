using Microsoft.AspNetCore.Mvc;

namespace Project1.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
