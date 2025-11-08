using SalonManagement.API.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Core/Interfaces/IUnitOfWork.cs
namespace SalonManagement.API.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern for transaction management
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        ISalonRepository Salons { get; }
        IEmployeeRepository Employees { get; }
        ICustomerRepository Customers { get; }
        IServiceRepository Services { get; }
        IAppointmentRepository Appointments { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}