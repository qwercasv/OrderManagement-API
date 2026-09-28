using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementAPI.Models
{
    public class Product
    {
        
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;     
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.00, 10000.00)]
        public decimal CostPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; } = 10;

        public bool isActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
    }
}
