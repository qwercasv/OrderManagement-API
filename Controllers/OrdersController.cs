using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Product).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound(new { message = $"Order ID {id} not found." });
            }
            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            if (orderDto.Items == null || !orderDto.Items.Any())
            {
                return BadRequest(new { message = "An order must contain at least one item" });
            }

            var newOrder = new Order
            {
                CustomerEmail = orderDto.CustomerEmail,
                Status = OrStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            decimal calculatedTotal = 0;

            foreach (var itemDto in orderDto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null || !product.isActive)
                {
                    return BadRequest(new { message = $"Product ID {itemDto.ProductId} is not found or inactive." });
                }

                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for product ID {itemDto.ProductId}. Available stock: {product.Stock}" });
                }

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                calculatedTotal += orderItem.UnitPrice * orderItem.Quantity;
                newOrder.Items.Add(orderItem);

                product.Stock -= itemDto.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            newOrder.TotalAmount = calculatedTotal;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Order ID {id} not found." });
            }

            if (order.Status == OrStatus.Cancelled)
            {
                return BadRequest(new { message = "Cannot change status of a cancelled order." });
            }

            order.Status = statusDto.NewStatus;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Order status updated successfully to {order.Status}" });
        }
    }
}
