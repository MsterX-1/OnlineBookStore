using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        public AuthService(IUserRepository userRepo,IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        private async Task<JwtSecurityToken> CreateJwtTokenAsync(User user)
        {
            // Get user roles
            var role = user.Role;
            
            // Standard JWT claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,$"{user.User_ID}"),
                new Claim("Role",user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
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





        public async Task<int> Register(RegisterDto dto)
        {
            // Check if username already exists
            var existingUser = await _userRepo.GetUserByUserNameAsync(dto.Username);
            if (existingUser != null)
                throw new Exception("Username already exists.");

            var user = dto.ConvertToUser();
            return await _userRepo.CreateUserAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                throw new Exception("Invalid username or password.");

            // Generate new JWT token
            var jwtToken = await CreateJwtTokenAsync(user);

            var response = new AuthResponseDto
            {
                Message = "Login successful",
                IsAuthenticated = true,
                Userid = user.User_ID,
                Username = user.Username,
                First_Name = user.First_Name,
                Last_Name = user.Last_Name,
                Email = user.Email,
                Role = user.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                TokenExpiresOn = jwtToken.ValidTo
            };
            return response;
        }
    }
}
