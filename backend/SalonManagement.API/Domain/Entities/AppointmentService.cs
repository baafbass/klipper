// SalonManagement.Core/Entities/AppointmentService.cs
namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// Services included in an appointment
    /// </summary>
    public class AppointmentService : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid ServiceId { get; private set; }
        public decimal Price { get; private set; }
        public int DurationMinutes { get; private set; }

        // Navigation properties
        public virtual Appointment Appointment { get; private set; }
        public virtual Service Service { get; private set; }

        private AppointmentService() { }

        public AppointmentService(Guid appointmentId, Guid serviceId, decimal price, int durationMinutes)
        {
            AppointmentId = appointmentId;
            ServiceId = serviceId;
            Price = price;
            DurationMinutes = durationMinutes;
        }
    }
}