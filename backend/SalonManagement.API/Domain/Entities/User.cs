// SalonManagement.Core/Entities/User.cs
namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// Base User class demonstrating Inheritance and Polymorphism
    /// </summary>
    public abstract class User : BaseEntity
    {
        public string Email { get; protected set; }
        public string PasswordHash { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string PhoneNumber { get; protected set; }
        public UserRole Role { get; protected set; }
        public bool IsActive { get; protected set; }

        protected User() { }

        protected User(string email, string firstName, string lastName,
                       string phoneNumber, UserRole role)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Role = role;
            IsActive = true;
        }

        public virtual string GetFullName() => $"{FirstName} {LastName}";

        public void SetPassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty");

            PasswordHash = passwordHash;
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

        public void UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            MarkAsUpdated();
        }
    }

    public enum UserRole
    {
        Customer = 1,
        Employee = 2,
        SalonManager = 3,
        SystemAdmin = 4
    }
}