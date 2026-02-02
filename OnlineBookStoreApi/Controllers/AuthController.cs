using System.Security.Claims;
using Application.Dtos.AuthDto;
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
        private readonly UserService _userService;
        private readonly ShoppingCartService _cartService;
        public AuthController(AuthService authService, UserService userService, ShoppingCartService cartService)
        {
            _authService = authService;
            _userService = userService;
            _cartService = cartService;
        }

        /// <summary>
        /// Sets refresh token as HTTP-only cookie for security
        /// HTTP-only cookies cannot be accessed by JavaScript, preventing XSS attacks
        /// </summary>
        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,           // Cannot be accessed by JavaScript (XSS protection)
                Expires = expires,         // Cookie expires with the refresh token
                Secure = true,             // Only sent over HTTPS (set to true in production)
                IsEssential = true         // Required for functionality
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
                var response = new AuthResponseDto
                {
                    Token = result.Token,
                    TokenExpiresOn = result.TokenExpiresOn
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        // No need to login again after registration already sets tokens
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.Register(dto);
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
                var response = new AuthResponseDto
                {
                    Token = result.Token,
                    TokenExpiresOn = result.TokenExpiresOn
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                {
                    return Unauthorized("Refresh token cookie not found.");
                }
                var result = await _authService.RefreshTokenAsync(refreshToken);
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
                var response = new AuthResponseDto
                {
                    Token = result.Token,
                    TokenExpiresOn = result.TokenExpiresOn
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("User ID claim not found.");
                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("Invalid User ID claim.");

                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("User ID claim not found.");
                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("Invalid User ID claim.");

                // clear customer cart
                if(!await _cartService.ClearCustomerCartAsync(userId))
                    return BadRequest("Failed to clear shopping cart during logout.");

                var result = await _authService.LogoutAsync(userId);
                if (result)
                {
                    // Remove the refresh token cookie
                    Response.Cookies.Delete("refreshToken");
                    return Ok("Logged out successfully.");
                }
                else
                {
                    return BadRequest("Logout failed.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
