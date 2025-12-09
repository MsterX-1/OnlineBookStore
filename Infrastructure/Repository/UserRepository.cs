using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using Dapper;

namespace Infrastructure.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IUserRepository Interface
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Users";
            return await db.QueryAsync<User>(sql);
        }

    }
}
