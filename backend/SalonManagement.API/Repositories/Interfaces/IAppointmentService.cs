using SalonManagement.API.Domain.Common;
using SalonManagement.API.DTOs;

namespace SalonManagement.API.Repositories.Interfaces
{
    public interface IAppointmentService
    {
        // Customer-facing
        Task<Result<IEnumerable<AppointmentDto>>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default);
        Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Availability: returns available slots for given salon/date/services (employee nullable -> search across employees)
        Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailabilityAsync(AvailabilityRequestDto request, CancellationToken cancellationToken = default);

        Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);

        // Approval / workflow
        Task<Result> ConfirmAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default); // manager or employee
        Task<Result> CancelAppointmentAsync(Guid appointmentId, string reason, CancellationToken cancellationToken = default);
    }
}
