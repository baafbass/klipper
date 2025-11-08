// SalonManagement.Infrastructure/Repositories/ServiceRepository.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Data;

namespace SalonManagement.API.Repositories.Implementations
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetBySalonIdAsync(Guid salonId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.SalonId == salonId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetActiveServicesBySalonAsync(Guid salonId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.SalonId == salonId && s.IsActive)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.EmployeeServices.Any(es =>
                    es.EmployeeId == employeeId &&
                    es.IsActive))
                .ToListAsync(cancellationToken);
        }
    }
}
