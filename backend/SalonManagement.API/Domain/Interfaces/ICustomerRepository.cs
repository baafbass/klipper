using SalonManagement.API.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Core/Interfaces/ICustomerRepository.cs
namespace SalonManagement.API.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer> GetCustomerWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}