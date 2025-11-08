// SalonManagement.Infrastructure/Repositories/SalonRepository.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Data;

namespace SalonManagement.API.Repositories.Implementations
{
    public class SalonRepository : Repository<Salon>, ISalonRepository
    {
        public SalonRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Salon>> GetActiveSalonsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .Include(s => s.WorkingHours)
                .ToListAsync(cancellationToken);
        }

        public async Task<Salon> GetSalonWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Services.Where(sv => sv.IsActive))
                .Include(s => s.WorkingHours)
                .Include(s => s.Employees.Where(e => e.IsActive))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Salon>> SearchSalonsByCity(string city, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.IsActive && s.City.Contains(city))
                .Include(s => s.WorkingHours)
                .ToListAsync(cancellationToken);
        }
    }
}