using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Data;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Domain.Interfaces;

namespace SalonManagement.API.Repositories.Implementations
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetByEmployeeIdAsync(Guid employeeId, DateTime date, CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _dbSet
                .Where(a => a.EmployeeId == employeeId && a.AppointmentDate == dateOnly)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetBySalonIdAsync(Guid salonId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.SalonId == salonId && a.AppointmentDate >= startDate.Date && a.AppointmentDate <= endDate.Date)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Appointment> GetAppointmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .Include(a => a.Salon)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        /// <summary>
        /// Returns true if there is any appointment for the employee on the given date that overlaps [startTime, endTime).
        /// Excludes appointment with id == excludeAppointmentId when provided.
        /// </summary>
        public async Task<bool> HasConflictAsync(Guid employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null, CancellationToken cancellationToken = default)
        {
            var q = _dbSet.Where(a => a.EmployeeId == employeeId && a.AppointmentDate == date.Date);

            if (excludeAppointmentId.HasValue)
                q = q.Where(a => a.Id != excludeAppointmentId.Value);

            // Overlap exists if NOT (existing.End <= new.Start || existing.Start >= new.End)
            return await q.AnyAsync(a => !(a.EndTime <= startTime || a.StartTime >= endTime), cancellationToken);
        }
    }
}
