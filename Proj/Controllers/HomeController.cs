using Microsoft.AspNetCore.Mvc;
using Proj.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
           ViewBag.products = products;
            var categories = _context.Categories.ToList();
            ViewBag.categories = categories;

            return View();
        }
        public IActionResult FeaturedProducts()
        {
            // Fetch the top 3 most frequent product_id from the OrderItems table
            var topProducts = _context.OrderItems
                .GroupBy(oi => oi.ProductId) // Group by product_id
                .Select(group => new 
                {
                    ProductId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(g => g.Count) // Order by count in descending order
                .Take(3) // Get top 3
                .ToList();

            // Fetch the product details for the top 3 most frequent products
            var featuredProducts = _context.Products
                .Where(p => topProducts.Select(t => t.ProductId).Contains(p.Id)) // Filter products by the top 3 product_ids
                .ToList();

            // Pass the products to the view
            ViewBag.products = featuredProducts;
            var categories = _context.Categories.ToList();
            ViewBag.categories = categories;
            

            return View("Index"); // Or the appropriate view for your product list
        }
        
       
        public IActionResult Filter(string productName, decimal? minPrice, decimal? maxPrice)
        {
            // Start with the base query to get all products
            var productsQuery = _context.Products.AsQueryable();

            // Apply product name filter if provided
            if (!string.IsNullOrEmpty(productName))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(productName));
            }

            // Apply price range filters if provided
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Fetch the filtered products
            var filteredProducts = productsQuery.ToList();

            // Pass the filtered products and filter values to the view
            ViewBag.products = filteredProducts;
            ViewData["productName"] = productName;
            ViewData["minPrice"] = minPrice;
            ViewData["maxPrice"] = maxPrice;
            var categories = _context.Categories.ToList();
            ViewBag.categories = categories;
            return View("Index"); // Return the same index view with filtered products
        }
        public IActionResult Category(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index"); // Redirect to the main page if no category is selected
            }

            var categories = _context.Categories.ToList();
            ViewBag.categories = categories;

            // Fetch products filtered by the selected category
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToList();

            ViewBag.products = products;

            return View("Index"); // Return to the Index view but with filtered products
        }


    }
}