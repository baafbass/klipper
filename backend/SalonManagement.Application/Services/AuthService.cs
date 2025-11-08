// SalonManagement.Application/Services/AuthService.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SalonManagement.Application.DTOs;
using SalonManagement.Application.DTOs.Auth;
using SalonManagement.Application.Interfaces;
using SalonManagement.Core.Common;
using SalonManagement.Core.Entities;
using SalonManagement.Infrastructure.Data; // adjust namespace if your DbContext is elsewhere
using AutoMapper;
using BCrypt.Net;

namespace SalonManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null) return Result<LoginResponseDto>.Failure("Invalid credentials");

            var passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordMatches) return Result<LoginResponseDto>.Failure("Invalid credentials");

            var tokenResult = await GenerateTokenAsync(user, cancellationToken);
            if (!tokenResult.IsSuccess)
                return Result<LoginResponseDto>.Failure(tokenResult.Error);

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null, // implement refresh tokens if you want
                User = _mapper.Map<UserDto>(user)
            };

            return Result<LoginResponseDto>.Success(dto);
        }

        public async Task<Result<LoginResponseDto>> RegisterCustomerAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            // check existing email
            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (exists) return Result<LoginResponseDto>.Failure("Email already in use");

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                // set defaults as appropriate (Role, IsActive, etc.)
            };

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var tokenResult = await GenerateTokenAsync(user, cancellationToken);
            if (!tokenResult.IsSuccess) return Result<LoginResponseDto>.Failure(tokenResult.Error);

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null,
                User = _mapper.Map<UserDto>(user)
            };

            return Result<LoginResponseDto>.Success(dto);
        }

        public async Task<Result<string>> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var jwtSection = _configuration.GetSection("JwtSettings");
                var secret = jwtSection.GetValue<string>("Secret");
                var issuer = jwtSection.GetValue<string>("Issuer");
                var audience = jwtSection.GetValue<string>("Audience");
                var expiryMinutes = jwtSection.GetValue<int>("ExpiryMinutes", 60);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                    new Claim("userId", user.Id.ToString()),
                    new Claim("firstName", user.FirstName ?? string.Empty),
                    new Claim("lastName", user.LastName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    // add role claim if you have it: new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    signingCredentials: creds
                );

                var str = new JwtSecurityTokenHandler().WriteToken(token);
                return Result<string>.Success(str);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Failed to generate token: {ex.Message}");
            }
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) return Result<UserDto>.Failure("User not found");

            var dto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(dto);
        }
    }
}
