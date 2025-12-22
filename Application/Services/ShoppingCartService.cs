using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CartDto;
using Application.Extention;
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

			return cartItems;

		}
		public async Task<int> AddItemToCartAsync(AddToCartDto dto)
		{
			var book = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
			if (book == null)
				throw new Exception("Book not found.");

            //check if item already exists in cart
			var existingCartItem = await _cartRepo.GetCartItemByCustomerAndISBNAsync(dto.CustomerId, dto.ISBN);
			if (existingCartItem == null)
                return await _cartRepo.AddToCartAsync(dto.ConvertToShoppingCartModel());
            
            var shoppingCartItem = new ShoppingCart
            {
                Cart_ID = existingCartItem.Cart_ID,
                Customer_ID = existingCartItem.Customer_ID,
                ISBN = existingCartItem.ISBN,
                Quantity = existingCartItem.Quantity + dto.Quantity
            };
             await _cartRepo.UpdateCartItemAsync(shoppingCartItem);
			return existingCartItem.Cart_ID;

        }
		public async Task<bool> UpdateCartItemAsync(UpdateCartDto cartDto)
		{
			var cartItem = await _cartRepo.GetCartItemAsync(cartDto.CartId);
			if (cartItem == null)
				throw new Exception("Cart item not found.");
			cartItem.Quantity = cartDto.Quantity;
			return await _cartRepo.UpdateCartItemAsync(cartItem.ConvertToShoppingCartModel());
		}
		public async Task<bool> RemoveCartAsync(int cartId)
		{
			var cartItem = await _cartRepo.GetCartItemAsync(cartId);
			if (cartItem == null)
				throw new Exception("Cart not found.");
			return await _cartRepo.RemoveCartAsync(cartId);
		}
		public async Task<bool> ClearCustomerCartAsync(int customerId)
		{
			return await _cartRepo.ClearCartAsync(customerId);
		}
		
		
	}
}
		