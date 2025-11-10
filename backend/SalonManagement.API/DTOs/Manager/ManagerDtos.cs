namespace SalonManagement.API.DTOs.Manager
{
    // Working hours
    public class CreateWorkingHoursDto
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsOpen { get; set; } = true;
    }

    public class UpdateWorkingHoursDto
    {
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsOpen { get; set; }
    }

    // Services
    public class CreateServiceForSalonDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateServiceForSalonDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    // Employee
    public class CreateEmployeeDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal? CommissionRate { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal? CommissionRate { get; set; }
    }

    // Employee schedule
    public class CreateEmployeeScheduleDto
    {
        public Guid EmployeeId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateEmployeeScheduleDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    // EmployeeService link
    public class CreateEmployeeServiceDto
    {
        public Guid EmployeeId { get; set; }
        public Guid ServiceId { get; set; }
    }
}
