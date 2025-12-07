using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repository
{
    public class BookRepository:IBookRepository
    {
        private readonly DatabaseContext _context;

        public BookRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using (var db = _context.CreateConnection())
            {
                string sql = "SELECT * FROM Book";
                return await db.QueryAsync<Book>(sql);
            }

        }
    }
}
