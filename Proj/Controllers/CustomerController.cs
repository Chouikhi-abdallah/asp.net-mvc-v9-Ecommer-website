using Microsoft.AspNetCore.Mvc;
using Proj.Models;
using Proj.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Proj.Controllers
{
    public class CustomerController : Controller
    {
        private readonly EcommerceDbContext _context;

        public CustomerController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: Customer/CustomerHome
        public IActionResult CustomerHome(string search, int? categoryId, int page = 1)
        {
            int pageSize = 5; // Number of products per page
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Pass categories to the view for the dropdown
            ViewBag.Categories = _context.Categories.ToList();

            // Handle search filtering
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
                ViewBag.SearchTerm = search;
            }

            // Handle category filtering
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId;
            }

            // Get the total count of products
            int totalProducts = products.Count();

            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Get the products for the current page
            var productsOnPage = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(productsOnPage);
        }

        // GET: Customer/Cart
        public IActionResult Cart()
        {
            if (HttpContext.Session.GetString("UserRole") == "Customer")
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? throw new InvalidOperationException());

                // Include Product to ensure it's loaded
                var cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToList();

                // Calculate total price
                var total = cartItems
                    .Where(c => c.Product != null)
                    .Sum(c => c.Product.Price * c.Quantity);

                ViewBag.Total = total;
                return View(cartItems);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Customer/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (HttpContext.Session.GetString("UserRole") == "Customer")
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var existingCartItem = _context.Carts
                    .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                    _context.Update(existingCartItem);
                }
                else
                {
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _context.Carts.Add(cartItem);
                }
                _context.SaveChanges();
                return RedirectToAction("Cart");
            }
            else
            {
                TempData["LoginPrompt"] = "Please log in to add items to your cart.";
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Customer/UpdateCartItem
        [HttpPost]
        public IActionResult UpdateCartItem(int cartItemId, bool increase)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartItemId);
            if (cartItem != null)
            {
                // Update quantity
                if (increase)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Cart");
        }

        // POST: Customer/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartItemId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Cart");
        }

        // GET: Customer/Checkout
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString("UserRole") == "Customer")
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToList();

                // Calculate total price
                var total = cartItems
                    .Where(c => c.Product != null)
                    .Sum(c => c.Product.Price * c.Quantity);

                ViewBag.Total = total;
                return View(cartItems);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult ConsultProduct(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return View(product); // Pass product to the view
        }

        // POST: Customer/Checkout
        [HttpPost]
        public IActionResult CheckoutConfirmed()
        {
            // Implement order confirmation logic here, e.g., save order, clear cart
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();

            // Process order logic here...

            // Remove cart items after checkout
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            TempData["OrderConfirmation"] = "Your order has been placed successfully!";
            return RedirectToAction("CustomerHome");
        }
        public IActionResult OrderHistory()
        {
            var userIdString = HttpContext.Session.GetString("UserId"); // Get the UserId from the session
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if the user is not logged in
            }

            var userId = int.Parse(userIdString); // Convert userId from string to int

            var orderHistory = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate) // Sort orders by OrderDate in descending order
                .Include(o => o.OrderItems) // Include order items
                .ThenInclude(oi => oi.Product) // Ensure the related Product is loaded for each OrderItem
                .ToList();

            return View(orderHistory);
        }


        
    }
}
