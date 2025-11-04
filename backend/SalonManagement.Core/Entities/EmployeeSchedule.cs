// SalonManagement.Core/Entities/EmployeeSchedule.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Employee availability schedule
    /// </summary>
    public class EmployeeSchedule : BaseEntity
    {
        public Guid EmployeeId { get; private set; }
        public int DayOfWeek { get; private set; } // 0=Sunday, 6=Saturday
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual Employee Employee { get; private set; }

        private EmployeeSchedule() { }

        public EmployeeSchedule(Guid employeeId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            if (dayOfWeek < 0 || dayOfWeek > 6)
                throw new ArgumentException("Day of week must be between 0 and 6", nameof(dayOfWeek));
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time");

            EmployeeId = employeeId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            IsActive = true;
        }

        public void UpdateSchedule(TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time");

            StartTime = startTime;
            EndTime = endTime;
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

        public bool IsAvailableAt(TimeSpan time)
        {
            return IsActive && time >= StartTime && time <= EndTime;
        }
    }
}