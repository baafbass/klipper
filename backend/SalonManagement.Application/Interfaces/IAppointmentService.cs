using SalonManagement.Application.DTOs;
using SalonManagement.Core.Common;
using System.Threading;
using System.Threading.Tasks;

// SalonManagement.Application/Interfaces/IAppointmentService.cs
namespace SalonManagement.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);
        //Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        //Task<Result<IEnumerable<AppointmentDto>>> GetCustomerAppointmentsAsync(Guid customerId, CancellationToken cancellationToken = default);
        //Task<Result<IEnumerable<AppointmentDto>>> GetEmployeeAppointmentsAsync(Guid employeeId, DateTime date, CancellationToken cancellationToken = default);
        //Task<Result<IEnumerable<AppointmentDto>>> GetSalonAppointmentsAsync(Guid salonId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        //Task<Result> ConfirmAppointmentAsync(Guid id, CancellationToken cancellationToken = default);
        //Task<Result> CancelAppointmentAsync(Guid id, string reason, CancellationToken cancellationToken = default);
        //Task<Result> CompleteAppointmentAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(AvailabilityRequestDto request, CancellationToken cancellationToken = default);
    }
}