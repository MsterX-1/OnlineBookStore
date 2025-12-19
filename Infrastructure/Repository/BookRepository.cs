using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Dapper;
using Application.Dtos.BookDto;

namespace Infrastructure.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DatabaseContext _context;

        public BookRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IBookRepository Interface

        public async Task<IEnumerable<GetBookDto>> GetAllBooksAsync()
        {
            using var db = _context.CreateConnection();
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year AS PubYear,b.Category,b.Price,b.Stock_Qty AS StockQty,
                        b.Threshold,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        FROM Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,p.Name
                        ORDER BY b.Title;";
            return await db.QueryAsync<GetBookDto>(sql);
        }

        public async Task<GetBookDto?> GetBookByISBNAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year AS PubYear,b.Category,b.Price,b.Stock_Qty AS StockQty,
                        b.Threshold,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        FROM Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Where b.ISBN=@ISBN
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,p.Name";
            return await db.QueryFirstOrDefaultAsync<GetBookDto>(sql, new { ISBN = isbn });
        }

        public async Task<IEnumerable<GetBookDto?>> GetBooksByCategoryAsync(string category)
        {
            using var db = _context.CreateConnection();
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year AS PubYear,b.Category,b.Price,b.Stock_Qty AS StockQty,
                        b.Threshold,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        FROM Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Where b.Category=@cat
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,p.Name
                        ORDER BY b.Title;";
            return await db.QueryAsync<GetBookDto>(sql, new { cat = category });
        }

        public async Task<IEnumerable<GetBookDto?>> SearchBooksByTitleAsync(string title)
        {
            using var db = _context.CreateConnection();
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year AS PubYear,b.Category,b.Price,b.Stock_Qty AS StockQty,
                        b.Threshold,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        FROM Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Where b.Title LIKE '%' + @Title + '%'
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,p.Name
                        ORDER BY b.Title;";
            return await db.QueryAsync<GetBookDto>(sql, new { Title = title });
        }

        public async Task<IEnumerable<GetBookDto?>> GetLowStockBooksAsync()
        {
            using var db = _context.CreateConnection();
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year AS PubYear,b.Category,b.Price,b.Stock_Qty AS StockQty,
                        b.Threshold,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        FROM Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Where b.Stock_Qty < b.Threshold
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,p.Name
                        ORDER BY b.Stock_Qty ASC, b.Title;";
            return await db.QueryAsync<GetBookDto>(sql);
        }
        public async Task<bool> CreateBookAsync(Book book)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                INSERT INTO Book (ISBN, Title, Pub_Year, Price, Category, Stock_Qty, Threshold, Publisher_ID)
                VALUES (@ISBN, @Title, @Pub_Year, @Price, @Category, @Stock_Qty, @Threshold, @Publisher_ID)";
            var rows = await db.ExecuteAsync(sql, book);
            return rows > 0;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                UPDATE Book 
                SET Title = @Title, 
                    Pub_Year = @Pub_Year, 
                    Price = @Price, 
                    Category = @Category, 
                    Stock_Qty = @Stock_Qty, 
                    Threshold = @Threshold, 
                    Publisher_ID = @Publisher_ID
                WHERE ISBN = @ISBN";
            var rows = await db.ExecuteAsync(sql, book);
            return rows > 0;
        }

        public async Task<Book?> GetBookEntityByISBNAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Book WHERE ISBN = @ISBN";
            return await db.QueryFirstOrDefaultAsync<Book>(sql, new { ISBN = isbn });
        }

        public async Task<bool> DeleteBookAsync(string isbn)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Book WHERE ISBN = @ISBN";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn });
            return rows > 0;
        }

        public async Task<bool> AddBookAuthorsAsync(string isbn, int authorId)
        {
            using var db = _context.CreateConnection();
            var sql = "INSERT INTO Book_Author (ISBN, Author_ID) VALUES (@ISBN, @Author_ID)";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn, Author_ID = authorId });
            return rows > 0;
        }

        public async Task<bool> RemoveBookAuthorsAsync(string isbn, int authorId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Book_Author WHERE ISBN = @ISBN AND Author_ID = @Author_ID";
            var rows = await db.ExecuteAsync(sql, new { ISBN = isbn, Author_ID = authorId });
            return rows > 0;
        }
        public async Task<IEnumerable<GetBookDto>> SearchBooksAdvancedAsync(SearchBookDto dto)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                SELECT 
                    b.ISBN,
                    b.Title,
                    b.Pub_Year AS PubYear,
                    b.Category,
                    b.Price,
                    b.Stock_Qty AS StockQty,
                    b.Threshold,
                    b.Publisher_ID AS PublisherId,
                    p.Name AS PublisherName,
                    STRING_AGG(a.Name, ',') AS Authors
                FROM Book b
                LEFT JOIN Publisher p ON b.Publisher_ID = p.Publisher_ID
                LEFT JOIN Book_Author ba ON b.ISBN = ba.ISBN
                LEFT JOIN Author a ON ba.Author_ID = a.Author_ID
                WHERE 
                    (@ISBN IS NULL OR b.ISBN LIKE '%' + @ISBN + '%')
                    AND (@Title IS NULL OR b.Title LIKE '%' + @Title + '%')
                    AND (@Category IS NULL OR b.Category = @Category)
                    AND (@PublisherName IS NULL OR p.Name LIKE '%' + @PublisherName + '%')
                    AND (@AuthorName IS NULL OR a.Name LIKE '%' + @AuthorName + '%')
                GROUP BY b.ISBN, b.Title, b.Pub_Year, b.Category, b.Price, 
                         b.Stock_Qty, b.Threshold, b.Publisher_ID, p.Name
                ORDER BY b.Title";

            return await db.QueryAsync<GetBookDto>(sql, dto);
        }
    }
}
