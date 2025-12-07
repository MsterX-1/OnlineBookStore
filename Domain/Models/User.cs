using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User
    {
        public int Userid { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string Phone { get; set; }
        public string Shipping_Address { get; set; }
        public  string User_Role { get; set; }
        public DateTime Created_At { get; set; }

    }
}
