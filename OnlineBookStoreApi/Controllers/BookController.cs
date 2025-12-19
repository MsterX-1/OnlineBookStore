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

        [HttpGet("GetBook/{isbn}")]
        public async Task<IActionResult> GetBook(string isbn)
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

        [HttpGet("SearchBooks/{title}")]
        public async Task<IActionResult> SearchBooks(string title)
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

        [HttpGet("BookAvailability/{isbn}")]
        public async Task<IActionResult> GetBookAvailability(string isbn)
        {
            try
            {
                var book = await _bookService.GetBookAvailabilityAsync(isbn);
                return Ok(book);
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
                await _bookService.CreateBookAsync(dto);
                return Ok(new { Message = "Book created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddBookAuthor")]
        public async Task<IActionResult> AddBookAuthor([FromQuery] string isbn, [FromQuery] int authorId)
        {
            try
            {
                await _bookService.AddBookAuthorAsync(isbn, authorId);
                return Ok(new { Message = "Author added to book successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SearchBooks")]
        public async Task<IActionResult> SearchBooks([FromBody] SearchBookDto searchDto)
        {
            try
            {
                var books = await _bookService.SearchBooksAsync(searchDto);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion

        #region Put Methods
        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDto dto)
        {
            try
            {
                await _bookService.UpdateBookAsync(dto);
                return Ok(new { Message = "Book updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateBookStock")]
        public async Task<IActionResult> UpdateBookStock([FromBody] UpdateBookStockDto dto)
        {
            try
            {
                await _bookService.UpdateBookStockAsync(dto);
                return Ok(new { Message = "Book stock updated successfully" });
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
                if (!result)
                {
                    return NotFound("Author or Book not found");
                }
                return Ok(new { Message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        [HttpDelete("RemoveBookAuthor")]
        public async Task<IActionResult> RemoveBookAuthor([FromQuery] string isbn, [FromQuery] int authorId)
        {
            try
            {
              var result = await _bookService.RemoveBookAuthorAsync(isbn, authorId);
                if(!result)
                {
                    return NotFound("Author or Book not found");
                }   
                return Ok(new { Message = "Author removed from book successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
