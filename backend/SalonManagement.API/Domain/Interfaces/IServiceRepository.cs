using SalonManagement.API.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Core/Interfaces/IServiceRepository.cs
namespace SalonManagement.API.Domain.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetBySalonIdAsync(Guid salonId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Service>> GetActiveServicesBySalonAsync(Guid salonId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Service>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    }
}