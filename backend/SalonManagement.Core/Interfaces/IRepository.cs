// SalonManagement.Core/Interfaces/IRepository.cs
namespace SalonManagement.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface - Repository Pattern
    /// Abstraction for data access (Dependency Inversion Principle)
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}