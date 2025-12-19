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
        // Implement The Methods Defined in IBookRepository Interface

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book";
            return await db.QueryAsync<Book>(sql);
        }

        public async Task<Book?> GetBookByISBNAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book WHERE ISBN = @ISBN";
            return await db.QueryFirstOrDefaultAsync<Book>(sql, new { ISBN = isbn });
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book WHERE Category = @Category";
            return await db.QueryAsync<Book>(sql, new { Category = category });
        }

        public async Task<IEnumerable<Book>> SearchBooksByTitleAsync(string title)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book WHERE Title LIKE @Title";
            return await db.QueryAsync<Book>(sql, new { Title = $"%{title}%" });
        }

        public async Task<IEnumerable<Book>> GetLowStockBooksAsync()
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book WHERE Stock_Qty < Threshold";
            return await db.QueryAsync<Book>(sql);
        }

        public async Task<bool> CreateBookAsync(Book book)
        {
            using var db = _context.CreateConnection();
            var sql = @"INSERT INTO Book (ISBN, Title, Pub_Year, Price, Category, Stock_Qty, Threshold, Publisher_ID)
                       VALUES (@ISBN, @Title, @Pub_Year, @Price, @Category, @Stock_Qty, @Threshold, @Publisher_ID)";
            var rows = await db.ExecuteAsync(sql, book);
            return rows > 0;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            using var db = _context.CreateConnection();
            var sql = @"UPDATE Book 
                       SET Title = @Title, Pub_Year = @Pub_Year, Price = @Price, 
                           Category = @Category, Stock_Qty = @Stock_Qty, 
                           Threshold = @Threshold, Publisher_ID = @Publisher_ID
                       WHERE ISBN = @ISBN";
            var rows = await db.ExecuteAsync(sql, book);
            return rows > 0;
        }

        public async Task<bool> UpdateBookStockAsync(string isbn, int newStock)
        {
            using var db = _context.CreateConnection();
            var sql = "UPDATE Book SET Stock_Qty = @NewStock WHERE ISBN = @ISBN";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn, NewStock = newStock });
            return rows > 0;
        }

        public async Task<bool> DeleteBookAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Book WHERE ISBN = @ISBN";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn });
            return rows > 0;
        }

        public async Task<IEnumerable<Author>> GetBookAuthorsAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = @"SELECT a.* FROM Author a
                       INNER JOIN Book_Author ba ON a.Author_ID = ba.Author_ID
                       WHERE ba.ISBN = @ISBN";
            return await db.QueryAsync<Author>(sql, new { ISBN = isbn });
        }

        public async Task<bool> AddBookAuthorAsync(string isbn, int authorId)
        {
            using var db = _context.CreateConnection();
            var sql = "INSERT INTO Book_Author (ISBN, Author_ID) VALUES (@ISBN, @AuthorId)";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn, AuthorId = authorId });
            return rows > 0;
        }

        public async Task<bool> RemoveBookAuthorAsync(string isbn, int authorId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Book_Author WHERE ISBN = @ISBN AND Author_ID = @AuthorId";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn, AuthorId = authorId });
            return rows > 0;
        }

        // neb2a n7otha fel PublisherRepository
        public async Task<string?> GetPublisherNameAsync(int publisherId)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT Name FROM Publisher WHERE Publisher_ID = @PublisherId";
            return await db.QueryFirstOrDefaultAsync<string>(sql, new { PublisherId = publisherId });
        }

    }
}
