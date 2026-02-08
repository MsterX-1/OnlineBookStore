using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuthDto;
using Application.Dtos.UserDto;
using Application.Extention;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        public AuthService(IUserRepository userRepo,IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
        {
            _userRepo = userRepo;
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
        }

        private JwtSecurityToken CreateJwtToken(User user)
        {
            // Get user roles
            var role = user.Role;
            
            // Standard JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,$"{user.User_ID}"),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            // Get JWT configuration from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JWT:DurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return token;
        }

        private RefreshToken GenerateRefreshToken(User user)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                User_ID = user.User_ID,
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["RefreshToken:DurationInDays"])),
                Created = DateTime.UtcNow
            };
        }

        public async Task<AuthResult> RefreshTokenAsync(string token)
        {
            var existingToken = await _refreshTokenRepo.GetRefreshTokenByTokenAsync(token);

            if (existingToken == null || !existingToken.IsActive)
                throw new Exception("Invalid or expired refresh token.");

            var user = await _userRepo.GetUserByIdAsync(existingToken.User_ID);
            if (user == null)
                throw new Exception("User not found.");

            // Generate new JWT and refresh token
            var jwtToken = CreateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user);

            // Save new token
            if (!await _refreshTokenRepo.SaveRefreshTokenAsync(newRefreshToken))
                throw new Exception("Failed to save refresh token.");

            // Revoke ONLY the current token (this device)
            // Other devices' tokens remain valid
            if (!await _refreshTokenRepo.RevokeRefreshTokenAsync(existingToken.Token))
                throw new Exception("Failed to revoke old token.");

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                TokenExpiresOn = jwtToken.ValidTo,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expires
            };
        }

        public async Task<AuthResult> Register(RegisterDto dto)
        {
            // Check if username already exists
            var existingUser = await _userRepo.GetUserByUserNameAsync(dto.Username);
            if (existingUser != null)
                throw new Exception("Username already exists.");

            var user = dto.ConvertToUser();
			user.Role = "Customer";

            // Create new User and it returns the new User ID
            var newUserID = await _userRepo.CreateUserAsync(user);

            user.User_ID = newUserID;
            // Generate new JWT token
            var jwtToken = CreateJwtToken(user);
            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken(user);
            // Save Refresh Token
            if (!await _refreshTokenRepo.SaveRefreshTokenAsync(refreshToken))
                throw new Exception("Failed to save refresh token.");

            var result = new AuthResult
			{
				Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
				TokenExpiresOn = jwtToken.ValidTo,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expires
            };
			return result;
		}

        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                throw new Exception("Invalid username or password.");

            var jwtToken = CreateJwtToken(user);

            //ALWAYS create a NEW refresh token for each login to support multiple device login and rotation
            var newRefreshToken = GenerateRefreshToken(user);

            if (!await _refreshTokenRepo.SaveRefreshTokenAsync(newRefreshToken))
                throw new Exception("Failed to save refresh token.");

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                TokenExpiresOn = jwtToken.ValidTo,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expires
            };
        }

        // Revokes all refresh tokens for a user (logout from all devices)
        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            // Revoke all active refresh tokens
            if(!await _refreshTokenRepo.RevokeAllUserRefreshTokensAsync(user.User_ID))
                throw new Exception("Failed to revoke refresh tokens.");

            return true;
        }
    }
}
