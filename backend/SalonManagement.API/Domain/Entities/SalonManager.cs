// SalonManagement.Core/Entities/SalonManager.cs
namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// SalonManager class - Higher privilege user
    /// </summary>
    public class SalonManager : User
    {
        public Guid SalonId { get; private set; }

        // Navigation properties
        public virtual Salon Salon { get; private set; }

        private SalonManager() { }

        public SalonManager(string email, string firstName, string lastName,
                           string phoneNumber, Guid salonId)
            : base(email, firstName, lastName, phoneNumber, UserRole.SalonManager)
        {
            SalonId = salonId;
        }

        public void AssignToSalon(Guid salonId)
        {
            SalonId = salonId;
            MarkAsUpdated();
        }
    }
}
