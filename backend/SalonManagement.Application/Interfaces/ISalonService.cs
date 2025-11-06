using SalonManagement.Application.DTOs;
using SalonManagement.Core.Common;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Application/Interfaces/ISalonService.cs
namespace SalonManagement.Application.Interfaces
{
    public interface ISalonService
    {
        Task<Result<IEnumerable<SalonDto>>> GetAllSalonsAsync(CancellationToken cancellationToken = default);
        Task<Result<SalonDto>> GetSalonByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<SalonDto>> CreateSalonAsync(CreateSalonDto dto, CancellationToken cancellationToken = default);
        Task<Result<SalonDto>> UpdateSalonAsync(Guid id, UpdateSalonDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteSalonAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<SalonDto>>> SearchSalonsByCityAsync(string city, CancellationToken cancellationToken = default);
    }
}