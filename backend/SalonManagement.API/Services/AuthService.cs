// SalonManagement.Application/Services/AuthService.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalonManagement.API.DTOs;
using SalonManagement.API.DTOs.Auth;
using SalonManagement.API.Repositories.Interfaces;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Data; // adjust namespace if your DbContext is elsewhere
using AutoMapper;
using BCrypt.Net;

namespace SalonManagement.API.Services
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
            var user = await _context.Customers
                .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null) return Result.Failure<LoginResponseDto>("Invalid credentials");

            var passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordMatches) return Result.Failure<LoginResponseDto>("Invalid credentials");

            var tokenResult = await GenerateTokenAsync(user, cancellationToken);
            if (!tokenResult.IsSuccess)
                return Result.Failure<LoginResponseDto>(tokenResult.Error ?? "Token generation failed");

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null,
                User = _mapper.Map<UserDto>(user)
            };

            return Result.Success(dto);
        }

        public async Task<Result<LoginResponseDto>> LoginSystemAdminAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _context.SystemAdmins
                .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null) return Result.Failure<LoginResponseDto>("Invalid credentials");

            var passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordMatches) return Result.Failure<LoginResponseDto>("Invalid credentials");

            var tokenResult = await GenerateTokenAsync(user, cancellationToken);
            if (!tokenResult.IsSuccess)
                return Result.Failure<LoginResponseDto>(tokenResult.Error ?? "Token generation failed");

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null,
                User = _mapper.Map<UserDto>(user)
            };

            return Result.Success(dto);
        }

        public async Task<Result<LoginResponseDto>> RegisterSystemAdminAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            // check existing email
            var exists = await _context.SystemAdmins.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (exists) return Result.Failure<LoginResponseDto>("Email already in use");

            // Use the public parameterized constructor (no object initializer)
            var systemAdmin = new SystemAdmin(
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber
            );

            // Set password via the provided method (encapsulated)
            var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password ?? string.Empty);
            systemAdmin.SetPassword(hashed);

            // Add and save
            _context.SystemAdmins.Add(systemAdmin);
            await _context.SaveChangesAsync(cancellationToken);

            var tokenResult = await GenerateTokenAsync(systemAdmin, cancellationToken);
            if (!tokenResult.IsSuccess) return Result.Failure<LoginResponseDto>(tokenResult.Error ?? "Token generation failed");

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null,
                User = _mapper.Map<UserDto>(systemAdmin)
            };

            return Result.Success(dto);
        }

        public async Task<Result<LoginResponseDto>> RegisterCustomerAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            // check existing email
            var exists = await _context.Customers.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (exists) return Result.Failure<LoginResponseDto>("Email already in use");

            // Use the public parameterized constructor (no object initializer)
            var customer = new Customer(
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.DateOfBirth
            );

            // Set password via the provided method (encapsulated)
            var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password ?? string.Empty);
            customer.SetPassword(hashed);

            // Add and save
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);

            var tokenResult = await GenerateTokenAsync(customer, cancellationToken);
            if (!tokenResult.IsSuccess) return Result.Failure<LoginResponseDto>(tokenResult.Error ?? "Token generation failed");

            var dto = new LoginResponseDto
            {
                Token = tokenResult.Value,
                RefreshToken = null,
                User = _mapper.Map<UserDto>(customer)
            };

            return Result.Success(dto);
        }


        public async Task<Result<string>> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var jwtSection = _configuration.GetSection("JwtSettings");
                var secret = jwtSection.GetValue<string>("Secret") ?? throw new InvalidOperationException("JWT secret missing");
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
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    signingCredentials: creds
                );

                var str = new JwtSecurityTokenHandler().WriteToken(token);
                return Result.Success(str);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Failed to generate token: {ex.Message}");
            }
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Customers.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) return Result.Failure<UserDto>("User not found");

            var dto = _mapper.Map<UserDto>(user);
            return Result.Success(dto);
        }
    }
}