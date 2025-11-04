// SalonManagement.Core/Interfaces/IUnitOfWork.cs
namespace SalonManagement.Core.Interfaces
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