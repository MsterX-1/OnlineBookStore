using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.UserDto;
using Application.Extention;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        // All the user related business logic will be implemented here
        public async Task<IEnumerable<GetUsersDto>> GetAllUsersAsync()
        {
            var users =  await _userRepo.GetAllUsersAsync();

            if (users == null || !users.Any())
                throw new Exception("No users found.");
  
            return users.ConvertToGetUsersDto();
        }

        public async Task<GetUserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                throw new Exception($"User with ID {id} not found.");
            return user.ConvertToGetUserDto();
        }
        public async Task<int> Register(RegisterDto dto)
        {
            // Check if username already exists
            var existingUser = await _userRepo.GetUserByUserNameAsync(dto.Username);
            if (existingUser != null)
                throw new Exception("Username already exists.");

            var user = dto.ConvertToUser();
            return await _userRepo.CreateUserAsync(user);
        }
        public async Task<GetUserDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                throw new Exception("Invalid username or password.");
            return user.ConvertToGetUserDto();
        }
        public async Task<bool> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception($"User with ID {dto.UserId} not found.");

            user.First_Name = dto.FirstName ?? user.First_Name;
            user.Last_Name = dto.LastName ?? user.Last_Name;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;

            var result = await _userRepo.UpdateUserAsync(user);
            if (!result)
                throw new Exception($"Failed to update user with ID {dto.UserId}.");
            return result;
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception($"User with ID {dto.UserId} not found.");

            // Verify old password
            if (user.Password != dto.OldPassword)
                throw new Exception("Old password is incorrect.");

            // Update to new password
            user.Password = dto.NewPassword;
            var result = await _userRepo.UpdateUserAsync(user);

            if (!result)
                throw new Exception("Failed to change password.");

            return result;
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            // Check if user exists
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            var result = await _userRepo.DeleteUserAsync(userId);
            if (!result)
                throw new Exception($"Failed to delete user with ID {userId}.");
            return result;
        }
    }
}
