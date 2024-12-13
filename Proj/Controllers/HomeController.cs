using Microsoft.AspNetCore.Mvc;
using Proj.Models;
using System.Linq;
using Proj.Data;

namespace Proj.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcommerceDbContext _context;

        public HomeController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}