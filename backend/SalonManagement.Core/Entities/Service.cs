// SalonManagement.Core/Entities/Service.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Service entity representing salon services
    /// </summary>
    public class Service : BaseEntity
    {
        public Guid SalonId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int DurationMinutes { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public string Category { get; private set; } // Haircut, Coloring, Shaving, etc.

        // Navigation properties
        public virtual Salon Salon { get; private set; }
        public virtual ICollection<EmployeeService> EmployeeServices { get; private set; }
        public virtual ICollection<AppointmentService> AppointmentServices { get; private set; }

        private Service()
        {
            EmployeeServices = new List<EmployeeService>();
            AppointmentServices = new List<AppointmentService>();
        }

        public Service(Guid salonId, string name, string description,
                      int durationMinutes, decimal price, string category)
        {
            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be positive", nameof(durationMinutes));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            SalonId = salonId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            DurationMinutes = durationMinutes;
            Price = price;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            IsActive = true;

            EmployeeServices = new List<EmployeeService>();
            AppointmentServices = new List<AppointmentService>();
        }

        public void UpdateDetails(string name, string description, int durationMinutes,
                                 decimal price, string category)
        {
            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be positive", nameof(durationMinutes));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            DurationMinutes = durationMinutes;
            Price = price;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }
    }
}