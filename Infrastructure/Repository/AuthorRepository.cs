using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AuthorRepository: IAuthorRepository
    {
        private readonly DatabaseContext _context;

        public AuthorRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorAsync()
        {
            // Implement The Methods Defined in IAuthorRepository Interface

            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Author";
            return await db.QueryAsync<Author>(sql);
        }

        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Author WHERE Author_ID = @Id";
            return await db.QueryFirstOrDefaultAsync<Author>(sql, new { Id = id });
        }
        public async Task<int> CreateAuthorAsync(string name)
        {
            using var db = _context.CreateConnection();
            var sql = @"INSERT INTO Author (Name)
                      VALUES (@Name)"
                       + "SELECT CAST(SCOPE_IDENTITY() as int)";
            return await db.ExecuteScalarAsync<int>(sql, new {Name = name });
        }
        public async Task<bool> DeleteAuthorAsync(int AuthorId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Author WHERE Author_ID = @AId";
            var result = await db.ExecuteAsync(sql, new { AId = AuthorId });
            return result > 0;
        }
        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            
                using var db = _context.CreateConnection();
                var sql = "UPDATE Author SET Name = @Name WHERE Author_ID = @Author_ID";
                var rows = await db.ExecuteAsync(sql, author);
                return rows > 0;
            

        }



    }
}
