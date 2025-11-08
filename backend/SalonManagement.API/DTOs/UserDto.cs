// SalonManagement.Application/DTOs/UserDto.cs
namespace SalonManagement.API.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeDto : UserDto
    {
        public Guid SalonId { get; set; }
        public string SalonName { get; set; }
        public decimal? CommissionRate { get; set; }
        public List<ServiceDto> Services { get; set; }
    }

    public class CustomerDto : UserDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string Notes { get; set; }
    }
}