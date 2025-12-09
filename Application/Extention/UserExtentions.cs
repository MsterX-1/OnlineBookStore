using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.UserDto;
using Domain.Models;

namespace Application.Extention
{
    public static class UserExtentions
    {
        public static IEnumerable<GetUsersDto> ConvertToGetUsersDto(this IEnumerable<User> users)
        {
            return users.Select(user => new GetUsersDto
            {
                Userid = user.User_ID,
                Username = user.Username,
                First_Name = user.First_Name,
                Last_Name = user.Last_Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            }).ToList();
        }
    }
}