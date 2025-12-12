using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.UserDto
{
    public class ChangePasswordDto
    {
        public int UserId { get; set; } // The ID of the user changing the password
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
