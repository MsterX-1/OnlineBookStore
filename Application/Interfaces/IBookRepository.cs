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
        Task<GetBookDto?> GetBookByISBNAsync(string isbn);
        Task<Book?> GetBookEntityByISBNAsync(string isbn);
        Task<IEnumerable<GetBookDto?>> GetBooksByCategoryAsync(string category);
        Task<IEnumerable<GetBookDto?>> SearchBooksByTitleAsync(string title);
        Task<IEnumerable<GetBookDto?>> GetLowStockBooksAsync();
        Task<bool> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(string isbn);
        Task<bool> AddBookAuthorsAsync(string isbn, int authorId);
        Task<bool> RemoveBookAuthorsAsync(string isbn, int authorId);
        Task<IEnumerable<GetBookDto>> SearchBooksAdvancedAsync(SearchBookDto dto);
    }
}
