using Microsoft.AspNetCore.Mvc;
using Proj.Data;
using Proj.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Proj.Controllers
{
    public class AdminController : Controller
    {
        private readonly EcommerceDbContext _context;

        public AdminController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/AdminDashboard
        public IActionResult AdminDashboard()
        {
            var totalUsers = _context.Users.Count() - 1; // Exclude admin
            var totalProducts = _context.Products.Count();
            var totalCategories = _context.Categories.Count();

            var products = _context.Products.ToList();
            var categories = _context.Categories.ToList();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.Products = products;
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Admin/ManageProducts
        public IActionResult ManageProducts()
        {
            var products = _context.Products.ToList();
            ViewBag.Products = products;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Admin/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Admin/AddProduct
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            if (imageFile.Length > 0)
            {
                // Set the image path with the '/images/' prefix
                product.ImagePath = "/images/" + Path.GetFileName(imageFile.FileName);
        
                var fileName = Path.GetFileName(imageFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        
                var filePath = Path.Combine(uploadsFolder, fileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageProducts");
        }

        // POST: /Admin/DeleteProduct
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                // Delete associated image file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageProducts");
        }

        // GET: /Admin/EditProduct
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
    
            if (product == null)
            {
                return NotFound();
            }

            // Optional: Ensure ImagePath is not null
            if (product.ImagePath == null)
            {
                product.ImagePath = "default-image.jpg"; // Set a default image if ImagePath is null
            }

            if (product.Category == null)
            {
                product.Category = new Category { Name = "Default Category" }; // Set a default category if null
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product); // Pass the product to the view
        }


// POST: /Admin/EditProduct
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product updatedProduct, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    updatedProduct.ImagePath = "/images/" + Path.GetFileName(imageFile.FileName); // Update the image path in the database
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                }

                _context.Products.Update(updatedProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProducts");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(updatedProduct);
        }

        // GET: /Admin/ManageCategory
        public IActionResult ManageCategory()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Admin/AddCategory
        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            if (ModelState.IsValid)
            {
                var category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("ManageCategory");
            }

            TempData["ErrorMessage"] = "There was an error adding the category.";
            return RedirectToAction("ManageCategory");
        }

        // POST: /Admin/EditCategory
        [HttpPost]
        public IActionResult EditCategory(int id, string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("ManageCategory");
            }

            category.Name = name;
            _context.Categories.Update(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category updated successfully!";
            return RedirectToAction("ManageCategory");
        }

        // POST: /Admin/DeleteCategory
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("ManageCategory");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction("ManageCategory");
        }
    }
}
