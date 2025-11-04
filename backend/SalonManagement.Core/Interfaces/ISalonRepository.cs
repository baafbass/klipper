// SalonManagement.Core/Interfaces/ISalonRepository.cs
namespace SalonManagement.Core.Interfaces
{
    public interface ISalonRepository : IRepository<Salon>
    {
        Task<IEnumerable<Salon>> GetActiveSalonsAsync(CancellationToken cancellationToken = default);
        Task<Salon> GetSalonWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Salon>> SearchSalonsByCity(string city, CancellationToken cancellationToken = default);
    }
}