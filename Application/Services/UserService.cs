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
    }
}
