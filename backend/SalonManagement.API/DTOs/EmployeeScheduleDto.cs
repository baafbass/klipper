// SalonManagement.Application/DTOs/EmployeeScheduleDto.cs
namespace SalonManagement.API.DTOs
{
    public class EmployeeScheduleDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateEmployeeScheduleDto
    {
        public Guid EmployeeId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}