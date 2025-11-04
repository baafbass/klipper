// SalonManagement.Application/Interfaces/IAuthService.cs
namespace SalonManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<LoginResponseDto>> RegisterCustomerAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<string>> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}