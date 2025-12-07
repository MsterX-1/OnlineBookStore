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
                Userid = user.Userid,
                Username = user.Username,
                Email = user.Email,
                User_Role = user.User_Role,
                Created_At = user.Created_At
            }).ToList();
        }
    }
}