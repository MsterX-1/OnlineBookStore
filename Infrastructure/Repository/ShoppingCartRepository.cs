using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CartDto;
using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class ShoppingCartRepository: IShoppingCartRepository
    {
        private readonly DatabaseContext _context;

        public ShoppingCartRepository(DatabaseContext context)
        {
            _context = context;
        }
       public async Task<IEnumerable<GetCartDto>> GetCartItemsByCustomerIdAsync(int customerId)
        {
            using var db = _context.CreateConnection();

            var sql = @"SELECT
                        sc.Cart_ID        AS CartId,
                        sc.Customer_ID    AS CustomerId,
                        sc.ISBN,
                        b.Title           AS BookTitle,
                        b.Price           AS UnitPrice,
                        sc.Quantity,
                        b.BookPhoto ,
                        (b.Price * sc.Quantity) AS TotalPrice
                    FROM Shopping_Cart sc
                    INNER JOIN Book b ON sc.ISBN = b.ISBN
                    WHERE sc.Customer_ID = @CustomerId;";

            var result = await db.QueryAsync<GetCartDto>(sql, new { CustomerId = customerId });

            if (!result.Any())
                throw new Exception("Cart is empty.");

            return result;
        }
        public async Task<GetCartDto?> GetCartItemAsync(int cartId)
        {
            using var db = _context.CreateConnection();
            var sql = @"SELECT
                        sc.Cart_ID        AS CartId,
                        sc.Customer_ID    AS CustomerId,
                        sc.ISBN,
                        b.Title           AS BookTitle,
                        b.Price           AS UnitPrice,
                        sc.Quantity,
                        (b.Price * sc.Quantity) AS TotalPrice
                    FROM Shopping_Cart sc
                    INNER JOIN Book b ON sc.ISBN = b.ISBN
                    WHERE sc.Cart_ID = @CartId;";
            return await db.QueryFirstOrDefaultAsync<GetCartDto>(sql, new { CartId = cartId });
		}
        public async Task<int> AddToCartAsync(ShoppingCart cartItem)
        {
            using var db = _context.CreateConnection();
            var sql = @"INSERT INTO Shopping_Cart (Customer_ID, ISBN, Quantity)
                       VALUES (@Customer_ID, @ISBN, @Quantity);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await db.ExecuteScalarAsync<int>(sql, cartItem);
		}
        public async Task<bool> UpdateCartItemAsync(ShoppingCart cartItem)
        {
            using var db = _context.CreateConnection();
            var sql = "UPDATE Shopping_Cart SET Customer_ID = @Customer_ID, ISBN = @ISBN, Quantity = @Quantity WHERE Cart_ID = @Cart_ID";
            var rows = await db.ExecuteAsync(sql, cartItem);
            return rows > 0;
        }
        public async Task<bool> RemoveCartAsync(int cartId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Shopping_Cart WHERE Cart_ID = @CartId";
            var rows = await db.ExecuteAsync(sql, new { CartId = cartId });
            return rows > 0;
        }
        public async Task<bool> ClearCartAsync(int customerId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Shopping_Cart WHERE Customer_ID = @CustomerId";
            var rows = await db.ExecuteAsync(sql, new { CustomerId = customerId });
            return rows > 0;
		}
        public async Task<ShoppingCart?> GetCartItemByCustomerAndISBNAsync(int customerId, string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Shopping_Cart WHERE Customer_ID = @CustomerId AND ISBN = @ISBN";
            return await db.QueryFirstOrDefaultAsync<ShoppingCart>(sql, new { CustomerId = customerId, ISBN = isbn });
		}
	}
}
