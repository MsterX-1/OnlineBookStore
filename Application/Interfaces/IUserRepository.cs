using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        // Define method signatures for user related repository operations
        public Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
