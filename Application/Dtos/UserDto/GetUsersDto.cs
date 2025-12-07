using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.UserDto
{
    public class GetUsersDto
    {
        public int Userid { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string User_Role { get; set; }
        public DateTime Created_At { get; set; }
    }
}
