// SalonManagement.Application/DTOs/AppointmentDto.cs
namespace SalonManagement.API.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid SalonId { get; set; }
        public string SalonName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalDurationMinutes { get; set; }
        public string Notes { get; set; }
        public List<AppointmentServiceDto> Services { get; set; }
    }

    public class CreateAppointmentDto
    {
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid SalonId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<Guid> ServiceIds { get; set; }
        public string Notes { get; set; }
    }

    public class AppointmentServiceDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class AvailabilityRequestDto
    {
        public Guid SalonId { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public List<Guid> ServiceIds { get; set; }
    }

    public class AvailableTimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}