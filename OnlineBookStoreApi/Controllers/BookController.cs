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
      //  #region Get Methods
        //[HttpGet("GetAllBooks")]
        //public async Task<IActionResult> GetAllBooks()
        //{
        //    try
        //    {
        //        var books = await _bookService.GetAllBooksAsync();
        //        return Ok(books);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}

    }
}
