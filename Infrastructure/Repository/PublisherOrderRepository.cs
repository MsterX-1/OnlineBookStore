using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class PublisherOrderRepository: IPublisherOrderRepository
    {
        private readonly DatabaseContext _context;

        public PublisherOrderRepository(DatabaseContext context)
        {
            _context = context;
        }
		public async Task<IEnumerable<PublisherOrder>> GetAllPublisherOrdersAsync()
        {

			using var db = _context.CreateConnection();
			var sql = "SELECT * FROM Publisher_Order ORDER BY Order_Date DESC";
			return await db.QueryAsync<PublisherOrder>(sql);
		}
		public async Task<PublisherOrder?> GetPublisherOrderByIdAsync(int pubOrderId)
		{
			using var db = _context.CreateConnection();
			var sql = "SELECT * FROM Publisher_Order WHERE Pub_Order_ID = @PubOrderId";
			return await db.QueryFirstOrDefaultAsync<PublisherOrder>(sql, new { PubOrderId = pubOrderId });
		}

		public async Task<IEnumerable<PublisherOrder>> GetPendingPublisherOrdersAsync()
		{
			using var db = _context.CreateConnection();
			var sql = "SELECT * FROM Publisher_Order WHERE Status = 'Pending' ORDER BY Order_Date";
			return await db.QueryAsync<PublisherOrder>(sql);
		}

		public async Task<int> CreatePublisherOrderAsync(PublisherOrder publisherOrder)
		{
			using var db = _context.CreateConnection();
			var sql = @"INSERT INTO Publisher_Order (ISBN, Quantity, Order_Date, Status)
                       VALUES (@ISBN, @Quantity, @Order_Date, @Status);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
			return await db.ExecuteScalarAsync<int>(sql, publisherOrder);
		}

		public async Task<bool> ConfirmPublisherOrderAsync(int pubOrderId)
		{
			using var db = _context.CreateConnection();
			var sql = "UPDATE Publisher_Order SET Status = 'Confirmed' WHERE Pub_Order_ID = @PubOrderId";
			var rows = await db.ExecuteAsync(sql, new { PubOrderId = pubOrderId });
			return rows > 0;
		}

	}
}
