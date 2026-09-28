using OrderManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrStatus NewStatus { get; set; }
    }
}
