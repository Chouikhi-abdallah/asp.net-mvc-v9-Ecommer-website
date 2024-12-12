using Microsoft.AspNetCore.Mvc;

namespace Proj.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            return View(); // Returns the default View (Index.cshtml) for the Home controller
        }
    }
}