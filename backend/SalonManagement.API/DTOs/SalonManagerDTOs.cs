// SalonManagement.API/DTOs/SalonManagerDtos.cs
namespace SalonManagement.API.DTOs
{
    public class SalonManagerRequestDto
    {
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        // Optionally require a password or generate one on server:
        public string? Password { get; set; }
    }

    public class SalonManagerDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public Guid SalonId { get; set; }
        public bool IsActive { get; set; }
    }
}
