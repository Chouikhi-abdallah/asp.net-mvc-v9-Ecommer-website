using Microsoft.AspNetCore.Mvc;
using Proj.Data;
using Proj.Models;
using System.Linq;

namespace Proj.Controllers
{
    public class AccountController : Controller
    {
        private readonly EcommerceDbContext _context;

        public AccountController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                ViewBag.ErrorMessage = "Please fill out all required fields correctly.";
                return View(user);
            }

            try
            {
                // Set default values
                user.Role = "Customer"; // Ensure the role is set
                user.Password = Models.User.HashPassword(user.Password); // Hash password
                user.Carts = new List<Cart>(); // Initialize empty carts collection

                // Save user
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Log and display error
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while registering. Please try again.";
                return View(user);
            }
        }




        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var hashedPassword = Models.User.HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user != null)
            {
                // Store user details in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Admin"); // Redirect admin to admin dashboard
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirect customers to the home page
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password";
            return View();
        }

        // Logout
        public IActionResult UpdateProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Account/UpdateProfile
        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            // Update user details
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = Models.User.HashPassword(updatedUser.Password);
            }

            _context.SaveChanges();

            ViewBag.SuccessMessage = "Profile updated successfully!";
            return View(user);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session
            return RedirectToAction("Login");
        }
    }
}
