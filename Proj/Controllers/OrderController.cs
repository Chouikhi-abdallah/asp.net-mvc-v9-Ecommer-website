using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proj.Models;
using Proj.Data;
using Proj.Services;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Proj.Controllers
{
    public class OrderController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly MailService _mailService; // Now injected via dependency injection

        public OrderController(EcommerceDbContext context, MailService mailService)
        {
            _context = context;
            _mailService = mailService; // Injected instance of MailService
        }

        // Show the checkout page with cart items and total price
        public IActionResult Checkout()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var totalAmount = cartItems
                .Where(c => c.Product != null)
                .Sum(c => c.Product.Price * c.Quantity);

            ViewBag.TotalAmount = totalAmount;
            return View(cartItems);
        }

        // Handle order confirmation after checkout
        [HttpPost]
        public IActionResult ConfirmOrder(string addressLine1, string addressLine2, string city, string state, string postalCode, string country)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                OrderStatus = "Pending",
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
                OrderDate = DateTime.Now  // Set the current date and time when the order is placed
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price
                };
                _context.OrderItems.Add(orderItem);
            }

            _context.SaveChanges();
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            // Send the email after order confirmation
            var subject = "Your Order is Now in Process";
            var message = $"Dear User, <br><br>Your order <strong>#{order.Id}</strong> has been confirmed and is now in process.<br><br>Total Amount: {order.TotalAmount.ToString("C")}<br><br>Thank you for shopping with us!";
            
            // Assuming the user email is stored in User table and fetched properly
            var userEmail = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefault(); // Replace with actual user's email address
            
            _mailService.SendEmailAsync(userEmail, subject, message).Wait(); // Send the email (you can also use async if needed)

            return RedirectToAction("StripePayment", new { orderId = order.Id });
        }

        // Initiate Stripe payment
        public IActionResult StripePayment(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var successUrl = Url.Action("OrderConfirmation", "Order", new { orderId }, protocol: Request.Scheme);
            var cancelUrl = Url.Action("Checkout", "Order", null, protocol: Request.Scheme);

            var stripeService = new StripePaymentService();
            var session = stripeService.CreateCheckoutSession(order.TotalAmount, successUrl, cancelUrl);

            return Redirect(session.Url);
        }

        // Display order confirmation
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
