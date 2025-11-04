// SalonManagement.Core/Entities/SalonWorkingHours.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Working hours for each salon by day of week
    /// </summary>
    public class SalonWorkingHours : BaseEntity
    {
        public Guid SalonId { get; private set; }
        public int DayOfWeek { get; private set; } // 0=Sunday, 6=Saturday
        public TimeSpan OpenTime { get; private set; }
        public TimeSpan CloseTime { get; private set; }
        public bool IsOpen { get; private set; }

        // Navigation properties
        public virtual Salon Salon { get; private set; }

        private SalonWorkingHours() { }

        public SalonWorkingHours(Guid salonId, int dayOfWeek, TimeSpan openTime, TimeSpan closeTime)
        {
            if (dayOfWeek < 0 || dayOfWeek > 6)
                throw new ArgumentException("Day of week must be between 0 and 6", nameof(dayOfWeek));
            if (closeTime <= openTime)
                throw new ArgumentException("Close time must be after open time");

            SalonId = salonId;
            DayOfWeek = dayOfWeek;
            OpenTime = openTime;
            CloseTime = closeTime;
            IsOpen = true;
        }

        public void UpdateHours(TimeSpan openTime, TimeSpan closeTime)
        {
            if (closeTime <= openTime)
                throw new ArgumentException("Close time must be after open time");

            OpenTime = openTime;
            CloseTime = closeTime;
            MarkAsUpdated();
        }

        public void SetOpen(bool isOpen)
        {
            IsOpen = isOpen;
            MarkAsUpdated();
        }
    }
}