using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserIdAsync(int UserId);
        Task <RefreshToken?> GetRefreshTokenByTokenAsync(string token);
        Task<bool> RevokeAllUserRefreshTokensAsync(int UserId);
        Task<bool> RevokeRefreshTokenAsync(string token);

    }
}
