using SalonManagement.API.DTOs;
using SalonManagement.API.DTOs.Auth;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Application/Interfaces/IAuthService.cs
namespace SalonManagement.API.Repositories.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<LoginResponseDto>> LoginSalonManagerAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<LoginResponseDto>> RegisterCustomerAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<LoginResponseDto>> LoginSystemAdminAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<LoginResponseDto>> RegisterSystemAdminAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<string>> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}