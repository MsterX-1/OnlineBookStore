using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IBookRepository
    {
        // Define method signatures for book repository operations
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByISBNAsync(string isbn);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
        Task<IEnumerable<Book>> SearchBooksByTitleAsync(string title);
        Task<IEnumerable<Book>> GetLowStockBooksAsync();
        Task<bool> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> UpdateBookStockAsync(string isbn, int newStock);
        Task<bool> DeleteBookAsync(string isbn);
        Task<IEnumerable<Author>> GetBookAuthorsAsync(string isbn);
        Task<bool> AddBookAuthorAsync(string isbn, int authorId);
        Task<bool> RemoveBookAuthorAsync(string isbn, int authorId);
        Task<string?> GetPublisherNameAsync(int publisherId);
    }
}
