using Application.Dtos.CartDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartService _cartService;

        public ShoppingCartController(ShoppingCartService cartService)
        {
            _cartService = cartService;
        }

		[HttpGet("GetCart/{customerId}")]
		public async Task<IActionResult> GetCart(int customerId)
		{
			try
			{
				var cartItems = await _cartService.GetCustomerCartItemsAsync(customerId);
				return Ok(cartItems);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("AddToCart")]
		public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
		{
			try
			{
				var cartId = await _cartService.AddItemToCartAsync(request);
				return Ok(new { CartId = cartId , Message = "Item added to cart successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	  [HttpPut("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartDto dto)
        {
            try
            {
                await _cartService.UpdateCartItemAsync(dto);
                return Ok(new { Message = "Cart item updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
		[HttpDelete("RemoveFromCart/{cartId}")]
		public async Task<IActionResult> RemoveFromCart(int cartId)
		{
			try
			{
				var result = await _cartService.RemoveItemFromCartAsync(cartId);
				if (result)
					return Ok(new { Message = "Item removed from cart successfully" });
				else
					return NotFound("Cart item not found.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpDelete("ClearCart/{customerId}")]
		public async Task<IActionResult> ClearCart(int customerId)
		{
			try
			{
				var result = await _cartService.ClearCustomerCartAsync(customerId);
				if (result)
					return Ok(new { Message = "Cart cleared successfully" });
				else
					return NotFound("Cart not found or already empty.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		
		
	}
}
