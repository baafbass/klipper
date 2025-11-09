namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// SystemAdmin class - Represents the top-level system administrator
    /// with the highest privileges to manage the platform.
    /// </summary>
    public class SystemAdmin : User
    {

        public DateTime CreatedAt { get; private set; }

        private SystemAdmin() { }

        public SystemAdmin(string email, string firstName, string lastName,
                           string phoneNumber)
            : base(email, firstName, lastName, phoneNumber, UserRole.SystemAdmin)
        {
            CreatedAt = DateTime.UtcNow;
        }

    }
}
