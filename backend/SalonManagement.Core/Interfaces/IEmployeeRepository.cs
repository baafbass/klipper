// SalonManagement.Core/Interfaces/IEmployeeRepository.cs
namespace SalonManagement.Core.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetBySalonIdAsync(Guid salonId, CancellationToken cancellationToken = default);
        Task<Employee> GetEmployeeWithSchedulesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(Guid salonId, DateTime date, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken = default);
        Task<bool> CanPerformServiceAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default);
    }
}