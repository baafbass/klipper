using SalonManagement.Application.DTOs;
using SalonManagement.Core.Common;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Application/Interfaces/IServiceService.cs
namespace SalonManagement.Application.Interfaces
{
    public interface IServiceService
    {
        Task<Result<IEnumerable<ServiceDto>>> GetServicesBySalonAsync(Guid salonId, CancellationToken cancellationToken = default);
        Task<Result<ServiceDto>> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ServiceDto>> CreateServiceAsync(CreateServiceDto dto, CancellationToken cancellationToken = default);
        Task<Result<ServiceDto>> UpdateServiceAsync(Guid id, UpdateServiceDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default);
    }
}