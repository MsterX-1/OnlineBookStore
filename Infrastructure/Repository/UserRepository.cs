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

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using var db = _context.CreateConnection(); 
            var sql = "SELECT * FROM Users WHERE User_ID = @Id";
            return await db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var db = _context.CreateConnection();
            var sql = @"INSERT INTO Users (Username, Password, First_Name, Last_Name, Email, Phone, Address)
                      VALUES (@Username, @Password, @First_Name, @Last_Name, @Email, @Phone, @Address)"
                        + "SELECT CAST(SCOPE_IDENTITY() as int)";   
            return await db.ExecuteScalarAsync<int>(sql, user);
           

        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            return await db.QueryFirstOrDefaultAsync<User>(sql, new { Username = userName });
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            using var db = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            return await db.QueryFirstOrDefaultAsync<User>(sql, new { Username = username, Password = password });
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
          using var db = _context.CreateConnection();
            var sql = @"UPDATE Users SET 
                        Username = @Username, 
                        Password = @Password, 
                        First_Name = @First_Name, 
                        Last_Name = @Last_Name, 
                        Email = @Email, 
                        Phone = @Phone, 
                        Address = @Address
                        WHERE User_ID = @User_ID";

            var result = await db.ExecuteAsync(sql, user);
            return result > 0;
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var db = _context.CreateConnection();
            var sql = "DELETE FROM Users WHERE User_ID = @UserId";
            var result = await db.ExecuteAsync(sql, new { UserId = userId });
            return result > 0;
        }

    }
}
