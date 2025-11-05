// SalonManagement.Infrastructure/Repositories/AppointmentRepository.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.Core.Entities;
using SalonManagement.Core.Interfaces;
using SalonManagement.Infrastructure.Data;

namespace SalonManagement.Infrastructure.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetByEmployeeIdAsync(Guid employeeId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.EmployeeId == employeeId && a.AppointmentDate == date.Date)
                .Include(a => a.Customer)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .OrderBy(a => a.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetBySalonIdAsync(Guid salonId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a =>
                    a.SalonId == salonId &&
                    a.AppointmentDate >= startDate.Date &&
                    a.AppointmentDate <= endDate.Date)
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<Appointment> GetAppointmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<bool> HasConflictAsync(
            Guid employeeId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeAppointmentId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.AppointmentDate == date.Date &&
                    (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed));

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            var appointments = await query.ToListAsync(cancellationToken);

            return appointments.Any(a =>
                !(a.EndTime <= startTime || a.StartTime >= endTime));
        }
    }
}