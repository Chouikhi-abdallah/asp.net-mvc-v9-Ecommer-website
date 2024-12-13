using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Proj.Models;
namespace Proj.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public string ImagePath { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")] // To handle price formatting
    public decimal Price { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }
}
