// SalonManagement.Core/Entities/EmployeeService.cs
namespace SalonManagement.Core.Entities
{
    /// <summary>
    /// Many-to-many relationship between Employee and Service
    /// </summary>
    public class EmployeeService : BaseEntity
    {
        public Guid EmployeeId { get; private set; }
        public Guid ServiceId { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual Employee Employee { get; private set; }
        public virtual Service Service { get; private set; }

        private EmployeeService() { }

        public EmployeeService(Guid employeeId, Guid serviceId)
        {
            EmployeeId = employeeId;
            ServiceId = serviceId;
            IsActive = true;
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
    }
}