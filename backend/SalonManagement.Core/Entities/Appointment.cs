// SalonManagement.Core/Entities/Appointment.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Appointment entity - Aggregate root for booking
    /// </summary>
    public class Appointment : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Guid SalonId { get; private set; }
        public DateTime AppointmentDate { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int TotalDurationMinutes { get; private set; }
        public string Notes { get; private set; }
        public string CancellationReason { get; private set; }

        // Navigation properties
        public virtual Customer Customer { get; private set; }
        public virtual Employee Employee { get; private set; }
        public virtual Salon Salon { get; private set; }
        public virtual ICollection<AppointmentService> AppointmentServices { get; private set; }

        private Appointment()
        {
            AppointmentServices = new List<AppointmentService>();
        }

        public Appointment(Guid customerId, Guid employeeId, Guid salonId,
                          DateTime appointmentDate, TimeSpan startTime)
        {
            CustomerId = customerId;
            EmployeeId = employeeId;
            SalonId = salonId;
            AppointmentDate = appointmentDate.Date;
            StartTime = startTime;
            Status = AppointmentStatus.Pending;
            TotalPrice = 0;
            TotalDurationMinutes = 0;

            AppointmentServices = new List<AppointmentService>();
        }

        public void AddService(Service service)
        {
            if (Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Cannot add services to non-pending appointment");

            var appointmentService = new AppointmentService(Id, service.Id, service.Price, service.DurationMinutes);
            AppointmentServices.Add(appointmentService);

            RecalculateTotals();
            MarkAsUpdated();
        }

        public void RemoveService(Guid serviceId)
        {
            if (Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Cannot remove services from non-pending appointment");

            var service = AppointmentServices.FirstOrDefault(s => s.ServiceId == serviceId);
            if (service != null)
            {
                AppointmentServices.Remove(service);
                RecalculateTotals();
                MarkAsUpdated();
            }
        }

        private void RecalculateTotals()
        {
            TotalPrice = AppointmentServices.Sum(s => s.Price);
            TotalDurationMinutes = AppointmentServices.Sum(s => s.DurationMinutes);
            EndTime = StartTime.Add(TimeSpan.FromMinutes(TotalDurationMinutes));
        }

        public void Confirm()
        {
            if (Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Only pending appointments can be confirmed");

            Status = AppointmentStatus.Confirmed;
            MarkAsUpdated();
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed appointments can be completed");

            Status = AppointmentStatus.Completed;
            MarkAsUpdated();
        }

        public void Cancel(string reason)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled appointment");

            Status = AppointmentStatus.Cancelled;
            CancellationReason = reason;
            MarkAsUpdated();
        }

        public void NoShow()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed appointments can be marked as no-show");

            Status = AppointmentStatus.NoShow;
            MarkAsUpdated();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            MarkAsUpdated();
        }

        public bool HasConflict(TimeSpan otherStart, TimeSpan otherEnd)
        {
            return !(EndTime <= otherStart || StartTime >= otherEnd);
        }
    }

    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4,
        NoShow = 5
    }
}