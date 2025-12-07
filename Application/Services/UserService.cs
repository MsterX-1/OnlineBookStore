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
        public async Task<IEnumerable<GetUsersDto>> GetAllUsersAsync()
        {
            var users =  await _userRepo.GetAllUsersAsync();

            if (users == null || !users.Any())
                throw new Exception("No users found.");

            return users.ConvertToGetUsersDto();
        }
    }
}
