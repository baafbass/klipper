namespace SalonManagement.API.DTOs.Employee
{
    public class EmployeeScheduleRequestDto
    {
        public int DayOfWeek { get; set; } // 0..6
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class EmployeeServiceAssignDto
    {
        public Guid ServiceId { get; set; }
    }
}
