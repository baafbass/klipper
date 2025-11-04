// SalonManagement.Core/Interfaces/IAppointmentRepository.cs
namespace SalonManagement.Core.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Appointment>> GetByEmployeeIdAsync(Guid employeeId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<Appointment>> GetBySalonIdAsync(Guid salonId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<Appointment> GetAppointmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> HasConflictAsync(Guid employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null, CancellationToken cancellationToken = default);
    }
}