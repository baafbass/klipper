// SalonManagement.Core/Entities/Salon.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Salon entity - Aggregate root
    /// </summary>
    public class Salon : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Employee> Employees { get; private set; }
        public virtual ICollection<SalonManager> Managers { get; private set; }
        public virtual ICollection<Service> Services { get; private set; }
        public virtual ICollection<SalonWorkingHours> WorkingHours { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }

        private Salon()
        {
            Employees = new List<Employee>();
            Managers = new List<SalonManager>();
            Services = new List<Service>();
            WorkingHours = new List<SalonWorkingHours>();
            Appointments = new List<Appointment>();
        }

        public Salon(string name, string address, string city,
                    string phoneNumber, string email)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            City = city ?? throw new ArgumentNullException(nameof(city));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            IsActive = true;

            Employees = new List<Employee>();
            Managers = new List<SalonManager>();
            Services = new List<Service>();
            WorkingHours = new List<SalonWorkingHours>();
            Appointments = new List<Appointment>();
        }

        public void UpdateInfo(string name, string description, string address,
                              string city, string phoneNumber, string email)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            City = city ?? throw new ArgumentNullException(nameof(city));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
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

        public bool IsOpenAt(DateTime dateTime)
        {
            var dayOfWeek = (int)dateTime.DayOfWeek;
            var time = dateTime.TimeOfDay;

            return WorkingHours.Any(wh =>
                wh.DayOfWeek == dayOfWeek &&
                wh.IsOpen &&
                wh.OpenTime <= time &&
                wh.CloseTime >= time);
        }
    }
}