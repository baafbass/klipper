// SalonManagement.Infrastructure/Repositories/EmployeeRepository.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Data;

namespace SalonManagement.API.Repositories.Implementations
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetBySalonIdAsync(Guid salonId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.SalonId == salonId && e.IsActive)
                .Include(e => e.EmployeeServices)
                    .ThenInclude(es => es.Service)
                .Include(e => e.Schedules)
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee> GetEmployeeWithSchedulesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(e => e.Schedules)
                .Include(e => e.EmployeeServices)
                    .ThenInclude(es => es.Service)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(
            Guid salonId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            CancellationToken cancellationToken = default)
        {
            var dayOfWeek = (int)date.DayOfWeek;

            return await _dbSet
                .Where(e => e.SalonId == salonId && e.IsActive)
                .Include(e => e.Schedules)
                .Include(e => e.Appointments)
                .Where(e => e.Schedules.Any(s =>
                    s.DayOfWeek == dayOfWeek &&
                    s.IsActive &&
                    s.StartTime <= startTime &&
                    s.EndTime >= endTime))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CanPerformServiceAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeServices
                .AnyAsync(es =>
                    es.EmployeeId == employeeId &&
                    es.ServiceId == serviceId &&
                    es.IsActive,
                    cancellationToken);
        }
    }
}