namespace SalonManagement.API.DTOs
{
    public class ServiceWithEmployeesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }

        public List<UserDto> Employees { get; set; } = new();
    }

    public class SalonDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public List<ServiceWithEmployeesDto> Services { get; set; } = new();
        public List<WorkingHoursDto> WorkingHours { get; set; } = new();
    }
}
