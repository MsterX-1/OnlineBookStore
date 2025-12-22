using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CartDto;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IShoppingCartRepository
    {
		Task<IEnumerable<GetCartDto>> GetCartItemsByCustomerIdAsync(int customerId);
		Task<GetCartDto?> GetCartItemAsync(int cartId);
		Task<int> AddToCartAsync(ShoppingCart cartItem);
		Task<bool> UpdateCartItemAsync(ShoppingCart cartItem);
		Task<bool> RemoveCartAsync(int cartId);
		Task<bool> ClearCartAsync(int customerId);
		Task<ShoppingCart?> GetCartItemByCustomerAndISBNAsync(int customerId, string isbn);
	}
}
