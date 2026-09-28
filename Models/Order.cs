using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementAPI.Models
{
    public enum OrStatus
    {
        Pending=0,
        Completed=1,
        Cancelled=2
    }
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        public OrStatus Status { get; set; } = OrStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    }
}
