using Application.Dtos.AuthorDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorService _authorService;

        public AuthorController(AuthorService authorService)
        {
            _authorService = authorService;
        }
        // All Endpoints are created for Author Entity
        [HttpGet("GetAllAuthors")]
        public async Task<IActionResult> GetAllAuthorsAsync()
        {
            try
            {
                var authors = await _authorService.GetAllAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAuthorById/{id}")]
        public async Task<IActionResult> GetAuthorByIdAsync(int id)
        {
            try
            {
                var author = await _authorService.GetAuthorByIdAsync(id);
                return Ok(author);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("CreateAuthor")]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] CreateAuthorDto Dto)
        {
            try
            {
                var authorId = await _authorService.CreateAuthorAsync(Dto);
                return Ok(new { AuthorId = authorId, Message = "Author created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("DeleteAuthor/{authorId}")]
        public async Task<IActionResult> DeleteAuthorAsync(int authorId)
        {
            try
            {
                var result = await _authorService.DeleteAuthorAsync(authorId);
                return Ok(new { Success = result, Message = "Author deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("updateauthor")]
        public async Task<IActionResult> UpdateAuthorAsync([FromBody] UpdateAuthorDTO dto)
        {
            try
            {
                var result =await _authorService.UpdateAuthorAsync(dto);
                return Ok(new { Success = result, Message = "Author updated successfully" });

            }
            catch  (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }



    }
}
