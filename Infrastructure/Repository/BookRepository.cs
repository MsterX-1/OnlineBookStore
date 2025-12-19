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
    public class BookRepository:IBookRepository
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
            var sql = @"Select b.ISBN,b.Title,b.Pub_Year,b.Category,b.Price,b.Stock_Qty,b.Threshold,b.Publisher_ID,p.Name As PublisherName,STRING_AGG(a.Name,',')As Authors
                        from Book as b
                        Left join Publisher p on b.Publisher_ID=p.Publisher_ID
                        Left join Book_Author ba on b.ISBN=ba.ISBN
                        Left join Author a on ba.Author_ID=a.Author_ID
                        Group by b.ISBN,b.Title,b.Pub_Year,b.Category,b.Publisher_ID,b.Price,b.Stock_Qty,b.Threshold,p.Name;";
            return await db.QueryAsync<GetBookDto>(sql);
        }

       

    }
}
