using Microsoft.AspNetCore.Mvc;
using Proj.Data;
using Proj.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Proj.Services;
using Microsoft.EntityFrameworkCore;

namespace Proj.Controllers
{
    public class AdminController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly MailService _mailService;

        public AdminController(EcommerceDbContext context, MailService mailService)
        {
            _context = context;
            _mailService = mailService;
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
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Product = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ImagePath = product.ImagePath,
                CategoryId = product.Category.Id
            };

            ViewBag.Categories = _context.Categories.Select(c => new { c.Id, c.Name }).ToList();

            return View();
        }





// POST: /Admin/EditProduct
[HttpPost]
public async Task<IActionResult> EditProduct(int id, string name, string description, decimal price, int quantity, int categoryId, IFormFile imageFile)
{
    var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    // Update product fields
    product.Name = name;
    product.Description = description;
    product.Price = Convert.ToDecimal(price);
    product.Quantity = quantity;
    product.CategoryId = categoryId;
    

    if (imageFile != null && imageFile.Length > 0)
    {
        // Save new image and update the path
        string newImagePath = "/images/" + Path.GetFileName(imageFile.FileName);
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        string filePath = Path.Combine(uploadsFolder, Path.GetFileName(imageFile.FileName));

        Directory.CreateDirectory(uploadsFolder);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Update image path in the database
        product.ImagePath = newImagePath;
    }
    _context.Products.Update(product);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Product updated successfully!";
    return RedirectToAction("ManageProducts");
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
        public IActionResult ManageUsers()
        {
            var users = _context.Users.ToList();
            return View(users); // Pass the list of users to the view
        }

        // Action to delete a user
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(); // Return a 404 error if the user is not found
            }

            _context.Users.Remove(user); // Remove the user from the database
            _context.SaveChanges(); // Save changes

            return RedirectToAction(nameof(ManageUsers)); // Redirect to the user list after deletion
        }
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            return View(orders);
        }

        // Show printable order details
        public IActionResult PrintOrder(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string orderStatus)
        {
            var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // Update the order status
            order.OrderStatus = orderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();

            // Ensure userEmail is not null
            string userEmail = order.User?.Email;
            if (string.IsNullOrEmpty(userEmail))
            {
                // Handle case where user email is missing
                return BadRequest("User email is missing.");
            }
           string subject = "Your Order Status Has Been Updated! 🎉";

            // Send email notification
            string message = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Order Status Updated</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2 style='color: #2d87f0;'>Dear {order.User?.Name},</h2>

    <p>We wanted to let you know that the status of your order <strong>#{order.Id}</strong> has been updated. Here's the latest update:</p>

    <h3>Current Status: <span style='color: #f79c42;'>{orderStatus}</span></h3>

    <hr style='border: 1px solid #ccc;'>

    <h4>🔹 Order Details:</h4>
    <ul>
        <li><strong>Order ID</strong>: #{order.Id}</li>
        <li><strong>Total Amount</strong>: {order.TotalAmount:C}</li>
        <li><strong>Products</strong>: 
            <ul>
                {string.Join("", (order.OrderItems ?? new List<OrderItem>()).Select(oi => $"<li>{oi.Product.Name} (x{oi.Quantity})</li>"))}
            </ul>
        </li>
    </ul>

    <hr style='border: 1px solid #ccc;'>

    <h4>What’s Next?</h4>
    <p>If your order is now marked as 'Shipped', you can expect it to arrive soon! You'll receive another email with your tracking details once your order is on the way.</p>

    <p>If there’s anything else we can assist you with, don’t hesitate to contact our support team at <strong>[Support Email]</strong> or visit our <a href='[Support Page]'>Support Page</a>.</p>

    <hr style='border: 1px solid #ccc;'>

    <p>Thank you for shopping with us!</p>

    <p>Best regards,<br>The <strong>AC E-COMMERCE</strong> Team</p>

    <hr style='border: 1px solid #ccc;'>
    <p><em>If you have any questions or need more information about your order, feel free to reach out. We're always happy to help!</em></p>

</body>
</html>
";

    
          _mailService.SendEmailAsync(userEmail, subject, message).Wait();

            TempData["SuccessMessage"] = "Order status updated successfully.";
            return RedirectToAction("Orders");
        }
        
        public IActionResult Statistics()
        {
            
            
            // Best Selling Products (Top 5)
            var bestSellingProducts = _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new 
                {
                    ProductName = g.First().Product.Name,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5)
                .ToList();

            // Products Out of Stock
            var outOfStockProducts = _context.Products
                .Where(p => p.Quantity == 0)
                .Select(p => new { p.Name, p.Id })
                .ToList();

            // Total Income
            var totalIncome = _context.Orders.Sum(o => o.TotalAmount);

            // Total Orders
            var totalOrders = _context.Orders.Count();

            // Total Products
            var totalProducts = _context.Products.Count();

            // Pass data to the view
            ViewBag.BestSellingProducts = bestSellingProducts;
            ViewBag.OutOfStockProducts = outOfStockProducts;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalProducts = totalProducts;

            return View();
        }


        
    }
}
