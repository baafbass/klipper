// SalonManagement.Core/Entities/Employee.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Employee class - Demonstrates composition and encapsulation
    /// </summary>
    public class Employee : User
    {
        public Guid SalonId { get; private set; }
        public string Specializations { get; private set; } // JSON array of specializations
        public decimal? CommissionRate { get; private set; }

        // Navigation properties
        public virtual Salon Salon { get; private set; }
        public virtual ICollection<EmployeeService> EmployeeServices { get; private set; }
        public virtual ICollection<EmployeeSchedule> Schedules { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }

        private Employee()
        {
            EmployeeServices = new List<EmployeeService>();
            Schedules = new List<EmployeeSchedule>();
            Appointments = new List<Appointment>();
        }

        public Employee(string email, string firstName, string lastName,
                       string phoneNumber, Guid salonId)
            : base(email, firstName, lastName, phoneNumber, UserRole.Employee)
        {
            SalonId = salonId;
            EmployeeServices = new List<EmployeeService>();
            Schedules = new List<EmployeeSchedule>();
            Appointments = new List<Appointment>();
        }

        public void AssignToSalon(Guid salonId)
        {
            SalonId = salonId;
            MarkAsUpdated();
        }

        public void SetCommissionRate(decimal rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentException("Commission rate must be between 0 and 100");

            CommissionRate = rate;
            MarkAsUpdated();
        }

        public void UpdateSpecializations(string specializations)
        {
            Specializations = specializations;
            MarkAsUpdated();
        }

        public bool CanPerformService(Guid serviceId)
        {
            return EmployeeServices.Any(es => es.ServiceId == serviceId && es.IsActive);
        }

        public bool IsAvailable(DateTime startTime, DateTime endTime)
        {
            var dayOfWeek = (int)startTime.DayOfWeek;

            return Schedules.Any(s =>
                s.DayOfWeek == dayOfWeek &&
                s.IsActive &&
                s.StartTime <= startTime.TimeOfDay &&
                s.EndTime >= endTime.TimeOfDay);
        }
    }
}