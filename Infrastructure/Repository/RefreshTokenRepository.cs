using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DatabaseContext _context;

        public RefreshTokenRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                SELECT User_ID, Token, Expires, Revoked, Created
                FROM RefreshToken
                WHERE Token = @token;";
            return await db.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token });
        }

        public async Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserIdAsync(int UserId)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                SELECT User_ID, Token, Expires, Revoked, Created
                FROM RefreshToken
                WHERE User_ID = @UserId
                ORDER BY Expires DESC;"; // to get the latest tokens first
            return await db.QueryAsync<RefreshToken>(sql, new { UserId });
        }

        public async Task<bool> RevokeAllUserRefreshTokensAsync(int UserId)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                UPDATE RefreshToken
                SET Revoked = GETUTCDATE()
                WHERE User_ID = @UserId 
                    AND Revoked IS NULL 
                    AND Expires > GETUTCDATE();"; // only revoke active tokens
            var result = await db.ExecuteAsync(sql, new { UserId });
            return result > 0;
        }

        public async Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            using var db = _context.CreateConnection();
            var sql = @"
                INSERT INTO RefreshToken (User_ID, Token, Expires, Created, Revoked)
                VALUES (@User_ID, @Token, @Expires, @Created, @Revoked);";
            var result = await db.ExecuteAsync(sql,refreshToken);
            return result > 0;


        }
    }
}
