using Application.Dtos.UserDto;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

		
		[HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
		[HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var respnse = await _authService.Register(dto);
                return Ok(respnse);
            }
			catch (Exception ex)
			{
				return BadRequest(new
				{
					error = ex.Message,
					inner = ex.InnerException?.Message,
					stack = ex.StackTrace
				});
			}

		}
	}
}
