using Application.Dtos.BookDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }
        // All Endpoints are created for Book Entity
         #region Get Methods
        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetBookByISBN/{isbn}")]
        public async Task<IActionResult> GetBookByISBN(string isbn)
        {
            try
            {
                var book = await _bookService.GetBookByISBNAsync(isbn);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetBooksByCategory/{category}")]
        public async Task<IActionResult> GetBooksByCategory(string category)
        {
            try
            {
                var books = await _bookService.GetBooksByCategoryAsync(category);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("SearchBooksByTitle/{title}")]
        public async Task<IActionResult> SearchBooksByTitle(string title)
        {
            try
            {
                var books = await _bookService.SearchBooksByTitleAsync(title);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetLowStockBooks")]
        public async Task<IActionResult> GetLowStockBooks()
        {
            try
            {
                var books = await _bookService.GetLowStockBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("SearchBooksAdvanced")]
        public async Task<IActionResult> SearchBooksAdvanced([FromBody] SearchBookDto dto)
        {
            try
            {
                var books = await _bookService.SearchBooksAdvancedAsync(dto);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Post Methods
        [HttpPost("CreateBook")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto dto)
        {
            try
            {
                var result = await _bookService.CreateBookAsync(dto);
                if (result)
                    return Ok("Book created successfully.");
                else
                    return BadRequest("Failed to create book.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddBookAuthors")]
        public async Task<IActionResult> AddBookAuthors([FromBody] AddOrRemoveBookAuthorsDto dto)
        {
            try
            {
                var result = await _bookService.AddBookAuthorsAsync(dto);
                if (result)
                    return Ok("Authors added to book successfully.");
                else
                    return BadRequest("Failed to add authors to book.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Put Methods
        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDto dto)
        {
            try
            {
                var result = await _bookService.UpdateBookAsync(dto);
                if (result)
                    return Ok("Book updated successfully.");
                else
                    return BadRequest("Failed to update book.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Delete Methods
        [HttpDelete("DeleteBook/{isbn}")]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(isbn);
                if (result)
                    return Ok("Book deleted successfully.");
                else
                    return BadRequest("Failed to delete book.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("RemoveBookAuthors")]
        public async Task<IActionResult> RemoveBookAuthors([FromBody] AddOrRemoveBookAuthorsDto dto)
        {
            try
            {
                var result = await _bookService.RemoveBookAuthorsAsync(dto);
                if (result)
                    return Ok("Authors removed from book successfully.");
                else
                    return BadRequest("Failed to remove authors from book.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
