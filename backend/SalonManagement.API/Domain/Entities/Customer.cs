// SalonManagement.API/Domain/Entities/Customer.cs
namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// Customer class - Inheritance from User
    /// </summary>
    public class Customer : User
    {
        public DateTime? DateOfBirth { get; private set; }
        public string? Notes { get; private set; }

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; private set; }

        private Customer()
        {
            Appointments = new List<Appointment>();
        }

        public Customer(string email, string firstName, string lastName,
                       string phoneNumber, DateTime? dateOfBirth = null)
            : base(email, firstName, lastName, phoneNumber, UserRole.Customer)
        {
            DateOfBirth = dateOfBirth;
            Appointments = new List<Appointment>();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            MarkAsUpdated();
        }

        public void UpdateDateOfBirth(DateTime dateOfBirth)
        {
            DateOfBirth = dateOfBirth;
            MarkAsUpdated();
        }
    }
}