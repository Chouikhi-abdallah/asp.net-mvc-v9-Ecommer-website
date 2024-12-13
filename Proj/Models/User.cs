using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Proj.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Admin" or "Customer"

        public virtual ICollection<Cart> Carts { get; set; }

        // Constructor to initialize Carts
        public User()
        {
            Carts = new List<Cart>(); // Initialize Carts to prevent validation errors
            Role = "Customer"; 
        }

        // Utility to hash passwords
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}