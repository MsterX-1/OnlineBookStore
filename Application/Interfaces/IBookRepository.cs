using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.BookDto;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IBookRepository
    {
        // Define method signatures for book repository operations
        Task<IEnumerable<GetBookDto>> GetAllBooksAsync();
        //Task<Book?> GetBookByISBNAsync(string isbn);
        //Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
        //Task<IEnumerable<Book>> SearchBooksByTitleAsync(string title);
        //Task<IEnumerable<Book>> GetLowStockBooksAsync();
        //Task<bool> CreateBookAsync(Book book);
        //Task<bool> UpdateBookAsync(Book book);
        //Task<bool> UpdateBookStockAsync(string isbn, int newStock);
    }
}
