using Application.Dtos.UserDto;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ShoppingCartService _cartService;
        private readonly AuthService _authService;
        public UserController(UserService userService, ShoppingCartService cartService, AuthService authService)
        {
            _userService = userService;
            _cartService = cartService;
            _authService = authService;
        }
        // All Endpoints are Created for User Entity

        #region Get Methods
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Put Methods
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            try
            {
                await _userService.UpdateUserAsync(dto);
                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // logout user after changing password
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                await _userService.ChangePasswordAsync(dto);
                var result = await _authService.LogoutAsync(dto.UserId);
                if (result)
                {
                    // Remove the refresh token cookie
                    Response.Cookies.Delete("refreshToken");
                    return Ok("Password changed successfully");
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
        #endregion

        #region Delete Methods
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


    }
}
