using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;
using Domain.Models;
using Dapper;
using Application.Dtos.OrderDto;
namespace Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _context;

        public OrderRepository(DatabaseContext context)
        {
            _context = context;
        }
		public async Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync()
		{
			using var db = _context.CreateConnection();

            var sql = @"SELECT 
						o.Order_ID,
						o.Customer_ID,
						o.Order_Date,
						o.Total_Amount,
						u.First_Name + ' ' + u.Last_Name AS CustomerName
						FROM Customer_Order o
						JOIN Users u ON o.Customer_ID = u.User_ID";
			return await db.QueryAsync<GetOrderDto>(sql);
        }

		public async Task<GetOrderDto?> GetOrderByIdAsync(int orderId)
		{
			using var db = _context.CreateConnection();

			 var sql = @"SELECT 
						o.Order_ID,
						o.Customer_ID,
						o.Order_Date,
						o.Total_Amount,
						u.First_Name + ' ' + u.Last_Name AS CustomerName
						FROM Customer_Order o
						JOIN Users u ON o.Customer_ID = u.User_ID
						WHERE o.Order_ID = @OrderID";
			return await db.QueryFirstOrDefaultAsync<GetOrderDto>(sql, new { OrderID = orderId });

        }

		public async Task<IEnumerable<GetOrderDto?>> GetOrdersByCustomerIdAsync(int customerId)
		{
			using var db = _context.CreateConnection();

			var sql = @"SELECT 
						o.Order_ID,
						o.Customer_ID,
						o.Order_Date,
						o.Total_Amount,
						u.First_Name + ' ' + u.Last_Name AS CustomerName
						FROM Customer_Order o
						JOIN Users u ON o.Customer_ID = u.User_ID
						WHERE o.Customer_ID = @CustomerId";
			return await db.QueryAsync<GetOrderDto>(sql, new { CustomerId = customerId });

        }

		public async Task<int> CreateOrderAsync(int customerId, string ccNumber, DateTime ccExpiry)
		{
			using var db = _context.CreateConnection();

			// Step 1: Insert order and get OrderId
			var sql = @"
						INSERT INTO Customer_Order (Customer_ID, Order_Date, Total_Amount, CC_Number, CC_Expiry)
						VALUES (
							@CustomerId,
							GETDATE(),
							(SELECT SUM(sc.Quantity * b.Price)
							 FROM Shopping_Cart sc
							 JOIN Book b ON sc.ISBN = b.ISBN
							 WHERE sc.Customer_ID = @CustomerId),
							@CCNumber,
							@CCExpiry);
					   SELECT CAST(SCOPE_IDENTITY() AS INT);";

			var orderId = await db.ExecuteScalarAsync<int>(sql, new
			{
				CustomerId = customerId,
				CCNumber = ccNumber,
				CCExpiry = ccExpiry
			});


			// Step 2: Insert order items
			await db.ExecuteAsync(@"
        INSERT INTO Order_Items (Order_ID, ISBN, Quantity, Unit_Price)
        SELECT
            @OrderId,
            sc.ISBN,
            sc.Quantity,
            b.Price
        FROM Shopping_Cart sc
        JOIN Book b ON sc.ISBN = b.ISBN
        WHERE sc.Customer_ID = @CustomerId;
    ", new { OrderId = orderId, CustomerId = customerId });

			// Step 3: Clear cart
			await db.ExecuteAsync(@"
        DELETE FROM Shopping_Cart
        WHERE Customer_ID = @CustomerId;
    ", new { CustomerId = customerId });

			return orderId;
		}

        public async Task<IEnumerable<GetOrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            using var db = _context.CreateConnection();
			var sql = @"
				SELECT Item_ID,ISBN,Quantity,Unit_Price
				FROM Order_Items
				WHERE Order_ID = @OrderId";
			return await db.QueryAsync<GetOrderItemDto>(sql, new { OrderId = orderId });
        }
    }
}
