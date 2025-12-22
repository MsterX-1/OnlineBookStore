using Application.Dtos.OrderDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
		[HttpGet("GetAllOrders")]
		public async Task<IActionResult> GetAllOrders()
		{
			try
			{
				var orders = await _orderService.GetAllOrdersAsync();
				return Ok(orders);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpGet("GetOrder/{orderId}")]
		public async Task<IActionResult> GetOrder(int orderId)
		{
			try
			{
				var order = await _orderService.GetOrderByIdAsync(orderId);
				return Ok(order);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpGet("GetOrdersByCustomer/{customerId}")]
		public async Task<IActionResult> GetOrdersByCustomer(int customerId)
		{
			try
			{
				var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
				return Ok(orders);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpGet("GetOrderDetails/{orderId}")]
		public async Task<IActionResult> GetOrderDetails(int orderId)
		{
			try
			{
				var orderDetails = await _orderService.GetOrderItemsByOrderIdAsync(orderId);
				return Ok(orderDetails);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
        }
        [HttpPost("PlaceOrder")]
		public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
		{
			try
			{
				var response = await _orderService.CreateOrderAsync(dto.CustomerId,dto.CCNumber,dto.CCExpiry);
				return Ok(new { OrderID = response, Messege = "Order Placed Successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
