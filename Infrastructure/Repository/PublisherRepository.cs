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
    public class PublisherRepository : IPublisherRepository
    {
        private readonly DatabaseContext _context;

        public PublisherRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Publisher";
            return await db.QueryAsync<Publisher>(sql);

        }
        public async Task<Publisher?> GetPublisherbyId(int Publisher_ID)
        {

            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Publisher where Publisher_ID=@Publisher_ID";
            return await db.QueryFirstOrDefaultAsync<Publisher>(sql, new { Publisher_ID = Publisher_ID });


        }
		public async Task<int> CreatePublisherAsync(Publisher publisher)
        {
			using var db = _context.CreateConnection();
			var sql = "INSERT INTO Publisher (Name, Address, Phone)" +
                       "VALUES (@Name, @Address, @Phone)" +
                       "SELECT CAST(SCOPE_IDENTITY() as int)";
			return await db.ExecuteScalarAsync<int>(sql, publisher);
		}


    }
}
