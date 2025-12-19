using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.BookDto;
using Application.Extention;
using Application.Interfaces;
using Domain.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace Application.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IPublisherRepository _publisherRepo;

        public BookService(IBookRepository bookRepo, IPublisherRepository publisherRepo)
        {
            _bookRepo = bookRepo;
            _publisherRepo = publisherRepo;
        }
        // All the book related business logic will be implemented here

        public async Task<IEnumerable<GetBookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepo.GetAllBooksAsync();
            //check if there are no books
            if (books == null || !books.Any())
                throw new Exception("No books found.");
            return books;
        }
        public async Task<GetBookDto?> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepo.GetBookByISBNAsync(isbn);
            if (book == null)
                throw new Exception($"Book with ISBN {isbn} not found.");
            return book;
        }
        public async Task<IEnumerable<GetBookDto?>> GetBooksByCategoryAsync(string category)
        {
            var books = await _bookRepo.GetBooksByCategoryAsync(category);
            if (books == null || !books.Any())
                throw new Exception($"No books found in category {category}.");
            return books;
        }
        public async Task<IEnumerable<GetBookDto?>> SearchBooksByTitleAsync(string title)
        {
            var books = await _bookRepo.SearchBooksByTitleAsync(title);
            if (books == null || !books.Any())
                throw new Exception($"No books found with title containing '{title}'.");
            return books;
        }
        public async Task<IEnumerable<GetBookDto?>> GetLowStockBooksAsync()
        {
            var books = await _bookRepo.GetLowStockBooksAsync();
            if (books == null || !books.Any())
                throw new Exception("No low stock books found.");
            return books;
        }
        public async Task<bool> CreateBookAsync(CreateBookDto dto)
        {
            // Check if book with the same ISBN already exists
            var existingBook = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
            if (existingBook != null)
                throw new Exception($"Book with ISBN {dto.ISBN} already exists.");
            // Check if publisher exists
            var publisher = await _publisherRepo.GetPublisherbyId(dto.PublisherId);
            if (publisher == null)
                throw new Exception($"Publisher with ID {dto.PublisherId} not found.");

            var book = dto.ToBookModel();
            var result = await _bookRepo.CreateBookAsync(book);
            if (!result)
                throw new Exception("Failed to create the book.");
            return result;
        }
        public async Task<bool> UpdateBookAsync(UpdateBookDto dto)
        {
            var book = await _bookRepo.GetBookEntityByISBNAsync(dto.ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {dto.ISBN} not found.");
            // Update book properties
            book.Title = dto.Title ?? book.Title;
            book.Pub_Year = dto.PubYear ?? book.Pub_Year;
            book.Category = dto.Category ?? book.Category;
            book.Price = dto.Price ?? book.Price;
            book.Stock_Qty = dto.StockQty ?? book.Stock_Qty;
            book.Threshold = dto.Threshold ?? book.Threshold;
            var result = await _bookRepo.UpdateBookAsync(book);
            if (!result)
                throw new Exception($"Failed to update book with ISBN {dto.ISBN}.");
            return result;

        }
        public async Task<bool> DeleteBookAsync(string isbn)
        {
            var book = await _bookRepo.GetBookEntityByISBNAsync(isbn);
            if (book == null)
                throw new Exception($"Book with ISBN {isbn} not found.");
            var result = await _bookRepo.DeleteBookAsync(isbn);
            if (!result)
                throw new Exception($"Failed to delete book with ISBN {isbn}.");
            return result;
        }
        public async Task<bool> AddBookAuthorsAsync(AddOrRemoveBookAuthorsDto dto)
        {
            var book = await _bookRepo.GetBookEntityByISBNAsync(dto.ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {dto.ISBN} not found.");
            var result = await _bookRepo.AddBookAuthorsAsync(dto.ISBN, dto.AuthorId);
            if (!result)
                throw new Exception($"Failed to add authors to book with ISBN {dto.ISBN}.");
            return result;
        }
        public async Task<bool> RemoveBookAuthorsAsync(AddOrRemoveBookAuthorsDto dto)
        {
            var book = await _bookRepo.GetBookEntityByISBNAsync(dto.ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {dto.ISBN} not found.");
            var result = await _bookRepo.RemoveBookAuthorsAsync(dto.ISBN, dto.AuthorId);
            if (!result)
                throw new Exception($"Failed to remove authors from book with ISBN {dto.ISBN}.");
            return result;
        }
        public async Task<IEnumerable<GetBookDto>> SearchBooksAdvancedAsync(SearchBookDto dto)
        {
            var books = await _bookRepo.SearchBooksAdvancedAsync(dto);
            if (books == null || !books.Any())
                throw new Exception("No books found matching the search criteria.");
            return books;
        }
        public async Task<bool> UploadBookPhotoAsync(string ISBN , byte[] photo)
        {
            var book = await _bookRepo.GetBookEntityByISBNAsync(ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {ISBN} not found.");
            var result = await _bookRepo.UbloadBookPhoto(ISBN,photo);
            if (!result)
                throw new Exception($"Failed to upload photo for book with ISBN {ISBN}.");
            return result;
        }

    }
}
