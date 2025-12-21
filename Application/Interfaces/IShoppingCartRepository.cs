using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IShoppingCartRepository
    {
		Task<IEnumerable<ShoppingCart>> GetCartItemsByCustomerIdAsync(int customerId);
		Task<ShoppingCart?> GetCartItemAsync(int cartId);
		Task<int> AddToCartAsync(ShoppingCart cartItem);
		Task<bool> UpdateCartItemAsync(ShoppingCart cartItem);
		Task<bool> RemoveFromCartAsync(int cartId);
		Task<bool> ClearCartAsync(int customerId);
		Task<ShoppingCart?> GetCartItemByCustomerAndISBNAsync(int customerId, string isbn);
	}
}
