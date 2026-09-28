using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public string TrackingNumber { get; set; } = Guid.NewGuid().ToString()[..8].ToUpper();
        public string Carrier { get; set; } = "Express Delivery";
        public string Status { get; set; } = "Ready for shipment";
        public DateTime EstimatedDelivery { get; set; } = DateTime.UtcNow.AddDays(3);
    }
}
