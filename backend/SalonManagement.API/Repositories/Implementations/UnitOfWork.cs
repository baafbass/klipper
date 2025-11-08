// SalonManagement.Infrastructure/Repositories/UnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Data;

namespace SalonManagement.API.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        private ISalonRepository _salons;
        private IEmployeeRepository _employees;
        private ICustomerRepository _customers;
        private IServiceRepository _services;
        private IAppointmentRepository _appointments;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ISalonRepository Salons =>
            _salons ??= new SalonRepository(_context);

        public IEmployeeRepository Employees =>
            _employees ??= new EmployeeRepository(_context);

        public ICustomerRepository Customers =>
            _customers ??= new CustomerRepository(_context);

        public IServiceRepository Services =>
            _services ??= new ServiceRepository(_context);

        public IAppointmentRepository Appointments =>
            _appointments ??= new AppointmentRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}