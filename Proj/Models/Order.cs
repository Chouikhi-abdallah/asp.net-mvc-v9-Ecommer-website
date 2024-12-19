using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }  // Link to User
        public virtual User User { get; set; }

        public decimal TotalAmount { get; set; }  // Total amount of the order
        public string OrderStatus { get; set; }  // E.g., "Pending", "Completed"

        // Shipping address
        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; }  // First line of the address

        [StringLength(200)]
        public string AddressLine2 { get; set; }  // Optional second line of the address

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; }  // Postal/ZIP code

        [Required]
        [StringLength(100)]
        public string Country { get; set; }  // Country name

        // Add OrderDate field
        [Required]
        public DateTime OrderDate { get; set; }  // Date and time the order was placed

        public virtual ICollection<OrderItem> OrderItems { get; set; }  // Link to OrderItems
    }
}