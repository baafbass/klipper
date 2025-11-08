// SalonManagement.Application/DTOs/ServiceDto.cs
namespace SalonManagement.API.DTOs
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public Guid SalonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateServiceDto
    {
        public Guid SalonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class UpdateServiceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}