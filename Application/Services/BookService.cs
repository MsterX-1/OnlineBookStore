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
            if (!books.Any())
                throw new Exception("No books found.");

            return await MapToGetBookDtosAsync(books);
        }

        public async Task<GetBookDto> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepo.GetBookByISBNAsync(isbn);
            if (book == null)
                throw new Exception($"Book with ISBN {isbn} not found.");

            return await MapToGetBookDtoAsync(book);
        }

        public async Task<IEnumerable<GetBookDto>> GetBooksByCategoryAsync(string category)
        {
            var books = await _bookRepo.GetBooksByCategoryAsync(category);
            if (!books.Any())
                throw new Exception($"No books found in category {category}.");

            return await MapToGetBookDtosAsync(books);

        }

        public async Task<IEnumerable<GetBookDto>> SearchBooksByTitleAsync(string title)
        {
            var books = await _bookRepo.SearchBooksByTitleAsync(title);
            if (!books.Any())
                throw new Exception($"No books found with title containing '{title}'.");

            return await MapToGetBookDtosAsync(books);
        }

        public async Task<IEnumerable<GetBookDto>> GetLowStockBooksAsync()
        {
            var books = await _bookRepo.GetLowStockBooksAsync();
            if (!books.Any())
                throw new Exception("No low stock books found.");

            return await MapToGetBookDtosAsync(books);
        }
        //Helper function convert single Book to GetBookDto
        private async Task<GetBookDto> MapToGetBookDtoAsync(Book book)
        {
            var publisherName = book.Publisher_ID.HasValue
                ? await _bookRepo.GetPublisherNameAsync(book.Publisher_ID.Value)
                : null;

            var authors = await _bookRepo.GetBookAuthorsAsync(book.ISBN);
            var authorNames = authors.Select(a => a.Name).ToList();

            return book.ConvertToGetBookDto(publisherName, authorNames);
        }
        // Helper function to convert list of Books to list of GetBookDtos
        private async Task<IEnumerable<GetBookDto>> MapToGetBookDtosAsync(IEnumerable<Book> books)
        {
            // iterate through books and convert each to GetBookDto asynchronously
            var tasks = books.Select(MapToGetBookDtoAsync);
            // Wait for all tasks to complete and return the results
            return await Task.WhenAll(tasks);
        }


        public async Task<bool> CreateBookAsync(CreateBookDto dto)
        {
            // Check if book with same ISBN already exists
            var existingBook = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
            if (existingBook != null)
                throw new Exception($"Book with ISBN {dto.ISBN} already exists.");
            // Check if Publisher exists if PublisherId is provided
                var publisher = await _publisherRepo.GetPublisherbyId(dto.PublisherId);
                if (publisher == null)
                    throw new Exception($"Publisher with ID {dto.PublisherId} does not exist.");
            
            var book = dto.ConvertToBook();
            var result = await _bookRepo.CreateBookAsync(book);

            // If authors are provided, associate them with the book else skip
            if (result && dto.AuthorIds != null && dto.AuthorIds.Any())
            {
                foreach (var authorId in dto.AuthorIds)
                {
                    await _bookRepo.AddBookAuthorAsync(dto.ISBN, authorId);
                }
            }
            return result;
        }

        public async Task<bool> UpdateBookAsync(UpdateBookDto dto)
        {
            // Check if book exists
            var book = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {dto.ISBN} does not exist.");
            // Check if Publisher exists if PublisherId is provided
            if (dto.PublisherId.HasValue)
            {
                var publisher = await _publisherRepo.GetPublisherbyId(dto.PublisherId.Value);
                if (publisher == null)
                    throw new Exception($"Publisher with ID {dto.PublisherId} does not exist.");
            }
            // Update book properties
            book.Title = dto.Title ?? book.Title;
            book.Pub_Year = dto.PubYear ?? book.Pub_Year;
            book.Price = dto.Price ?? book.Price;
            book.Category = dto.Category ?? book.Category;
            book.Stock_Qty = dto.StockQty ?? book.Stock_Qty;
            book.Threshold = dto.Threshold ?? book.Threshold;
            book.Publisher_ID = dto.PublisherId ?? book.Publisher_ID;

            var result = await _bookRepo.UpdateBookAsync(book);
            if (!result)
                throw new Exception($"Failed to update book with ISBN {dto.ISBN}.");
            return result;
        }

        //malha4 lazma awy momken asta3mel el update book we 5las
        public async Task<bool> UpdateBookStockAsync(UpdateBookStockDto dto)
        {
            // Check if book exists
            var book = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
            if (book == null)
                throw new Exception($"Book with ISBN {dto.ISBN} does not exist.");
            var result = await _bookRepo.UpdateBookStockAsync(dto.ISBN, dto.NewStock);
            if (!result)
                throw new Exception($"Failed to update stock for book with ISBN {dto.ISBN}.");
            return result;
        }

        public async Task<bool> DeleteBookAsync(string isbn)
        {
            // Check if book exists
            var book = await _bookRepo.GetBookByISBNAsync(isbn);
            if (book == null)
                throw new Exception($"Book with ISBN {isbn} does not exist.");
            var result = await _bookRepo.DeleteBookAsync(isbn);
            if (!result)
                throw new Exception($"Failed to delete book with ISBN {isbn}.");
            return result;
        }

        public async Task<bool> AddBookAuthorAsync(string isbn, int authorId)
        {
            // No need to check ther is a constraint at Database on Author_ID and ISBN
            return await _bookRepo.AddBookAuthorAsync(isbn, authorId);
        }

        public async Task<bool> RemoveBookAuthorAsync(string isbn, int authorId)
        {
            // No need to check
            return await _bookRepo.RemoveBookAuthorAsync(isbn, authorId);
        }
        public async Task<IEnumerable<BookAvailabilityDto>> SearchBooksAsync(SearchBookDto searchDto)
        {
            var books = await _bookRepo.GetAllBooksAsync();

            // Apply filters based on search criteria
            if (!string.IsNullOrEmpty(searchDto.ISBN))
            {
                books = books.Where(b => b.ISBN.Contains(searchDto.ISBN, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchDto.Title))
            {
                books = books.Where(b => b.Title.Contains(searchDto.Title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchDto.Category))
            {
                books = books.Where(b => b.Category != null &&
                    b.Category.Equals(searchDto.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchDto.PublisherName) && searchDto.PublisherName != "0")
            {
                // Filter by publisher - will need to join with publisher
                var filteredByPublisher = new List<Book>();
                foreach (var book in books)
                {
                    if (book.Publisher_ID.HasValue)
                    {
                        var publisherName = await _bookRepo.GetPublisherNameAsync(book.Publisher_ID.Value);
                        if (publisherName != null && publisherName.Contains(searchDto.PublisherName, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredByPublisher.Add(book);
                        }
                    }
                }
                books = filteredByPublisher;
            }

            if (!string.IsNullOrEmpty(searchDto.AuthorName))
            {
                // Filter by author
                var filteredByAuthor = new List<Book>();
                foreach (var book in books)
                {
                    var authors = await _bookRepo.GetBookAuthorsAsync(book.ISBN);
                    if (authors.Any(a => a.Name.Contains(searchDto.AuthorName, StringComparison.OrdinalIgnoreCase)))
                    {
                        filteredByAuthor.Add(book);
                    }
                }
                books = filteredByAuthor;
            }

            if (!books.Any())
                throw new Exception("No books found matching the search criteria.");

            // Convert to BookAvailabilityDto
            var bookDtos = new List<BookAvailabilityDto>();
            foreach (var book in books)
            {
                var publisherName = book.Publisher_ID.HasValue
                    ? await _bookRepo.GetPublisherNameAsync(book.Publisher_ID.Value)
                    : null;
                var authors = await _bookRepo.GetBookAuthorsAsync(book.ISBN);
                var authorNames = authors.Select(a => a.Name).ToList();

                bookDtos.Add(new BookAvailabilityDto
                {
                    ISBN = book.ISBN,
                    Title = book.Title,
                    PubYear = book.Pub_Year,
                    Price = book.Price,
                    Category = book.Category,
                    StockQty = book.Stock_Qty,
                    IsAvailable = book.Stock_Qty > 0,
                    PublisherName = publisherName,
                    Authors = authorNames
                });
            }

            return bookDtos;
        }

        public async Task<BookAvailabilityDto> GetBookAvailabilityAsync(string isbn)
        {
            var book = await _bookRepo.GetBookByISBNAsync(isbn);
            if (book == null)
                throw new Exception($"Book with ISBN {isbn} not found.");

            var publisherName = book.Publisher_ID.HasValue
                ? await _bookRepo.GetPublisherNameAsync(book.Publisher_ID.Value)
                : null;
            var authors = await _bookRepo.GetBookAuthorsAsync(book.ISBN);
            var authorNames = authors.Select(a => a.Name).ToList();

            return new BookAvailabilityDto
            {
                ISBN = book.ISBN,
                Title = book.Title,
                PubYear = book.Pub_Year,
                Price = book.Price,
                Category = book.Category,
                StockQty = book.Stock_Qty,
                IsAvailable = book.Stock_Qty > 0,
                PublisherName = publisherName,
                Authors = authorNames
            };
        }

    }
}
