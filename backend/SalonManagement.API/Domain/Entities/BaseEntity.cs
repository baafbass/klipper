// SalonManagement.Core/Entities/BaseEntity.cs
namespace SalonManagement.API.Domain.Entities
{
    /// <summary>
    /// Base entity class implementing common properties for all entities
    /// Encapsulation: Protected setters ensure controlled modification
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public virtual void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public virtual void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}