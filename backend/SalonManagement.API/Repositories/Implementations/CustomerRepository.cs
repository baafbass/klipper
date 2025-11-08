// SalonManagement.Infrastructure/Repositories/CustomerRepository.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Data;

namespace SalonManagement.API.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }

        public async Task<Customer> GetCustomerWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.AppointmentServices)
                        .ThenInclude(aps => aps.Service)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Employee)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Salon)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}