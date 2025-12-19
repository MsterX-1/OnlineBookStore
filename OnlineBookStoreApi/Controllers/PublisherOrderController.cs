using Application.Dtos.PublisherOrderDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherOrderController : ControllerBase
    {
        private readonly PublisherOrderService _pubOrderService;

        public PublisherOrderController(PublisherOrderService pubOrderService)
        {
            _pubOrderService = pubOrderService;
        }
		[HttpGet("GetAllPublisherOrders")]
		public async Task<IActionResult> GetAllPublisherOrders()
		{
			try
			{
				var orders = await _pubOrderService.GetAllPublisherOrdersAsync();
				return Ok(orders);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpGet("GetPendingOrders")]
		public async Task<IActionResult> GetPendingOrders()
		{
			try
			{
				var orders = await _pubOrderService.GetPendingPublisherOrdersAsync();
				return Ok(orders);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPost("CreatePublisherOrder")]
		public async Task<IActionResult> CreatePublisherOrder([FromBody] CreatePublisherOrderDto dto)
		{
			try
			{
				var orderId = await _pubOrderService.CreatePublisherOrderAsync(dto);
				return Ok(new { OrderId = orderId, Message = "Publisher order created successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("ConfirmOrder")]
		public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmPublisherOrderDto dto)
		{
			try
			{
				await _pubOrderService.ConfirmPublisherOrderAsync(dto);
				return Ok(new { Message = "Publisher order confirmed successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
