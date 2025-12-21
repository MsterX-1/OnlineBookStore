using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CartDto;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
	public class ShoppingCartService
	{
		private readonly IShoppingCartRepository _cartRepo;
		private readonly IBookRepository _bookRepo;

		public ShoppingCartService(IShoppingCartRepository cartRepo, IBookRepository bookRepo)
		{
			_cartRepo = cartRepo;
			_bookRepo = bookRepo;
		}
		public async Task<IEnumerable<GetCartDto>> GetCustomerCartItemsAsync(int customerId)
		{
			var cartItems = await _cartRepo.GetCartItemsByCustomerIdAsync(customerId);
			if (!cartItems.Any())
				throw new Exception("Cart is empty.");

			var cartDtos = new List<GetCartDto>();
			foreach (var item in cartItems)
			{
				var book = await _bookRepo.GetBookByISBNAsync(item.ISBN!);
				cartDtos.Add(new GetCartDto
				{
					CartId = item.Cart_ID,
					CustomerId = item.Customer_ID!.Value,
					ISBN = item.ISBN,
					BookTitle = book?.Title,
					UnitPrice = book?.Price,
					Quantity = item.Quantity!.Value,
					TotalPrice = book != null ? book.Price * item.Quantity!.Value : 0
				});
			}
			return cartDtos;

		}
		public async Task<int> AddItemToCartAsync(int customerId, string isbn, int quantity)
		{
			var book = await _bookRepo.GetBookByISBNAsync(isbn);
			if (book == null)
				throw new Exception("Book not found.");
			var cartItem = new ShoppingCart
			{
				Customer_ID = customerId,
				ISBN = isbn,
				Quantity = quantity
			};
			return await _cartRepo.AddToCartAsync(cartItem);

		}
		public async Task<bool> UpdateCartItemAsync(UpdateCartDto cartDto)
		{
			var cartItem = await _cartRepo.GetCartItemAsync(cartDto.CartId);
			if (cartItem == null)
				throw new Exception("Cart item not found.");
			cartItem.Quantity = cartDto.Quantity;
			return await _cartRepo.UpdateCartItemAsync(cartItem);
		}
		public async Task<bool> RemoveItemFromCartAsync(int cartId)
		{
			var cartItem = await _cartRepo.GetCartItemAsync(cartId);
			if (cartItem == null)
				throw new Exception("Cart item not found.");
			return await _cartRepo.RemoveFromCartAsync(cartId);
		}
		public async Task<bool> ClearCustomerCartAsync(int customerId)
		{
			return await _cartRepo.ClearCartAsync(customerId);
		}
		
		
	}
}
		