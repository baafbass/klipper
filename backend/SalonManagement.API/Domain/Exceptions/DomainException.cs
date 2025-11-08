// SalonManagement.Core/Exceptions/DomainException.cs
namespace SalonManagement.API.Domain.Exceptions
{
    /// <summary>
    /// Base exception for domain-level errors
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName} with ID {id} was not found") { }
    }

    public class AppointmentConflictException : DomainException
    {
        public AppointmentConflictException()
            : base("The selected time slot conflicts with an existing appointment") { }
    }

    public class EmployeeNotAvailableException : DomainException
    {
        public EmployeeNotAvailableException()
            : base("The selected employee is not available at the requested time") { }
    }

    public class SalonClosedException : DomainException
    {
        public SalonClosedException()
            : base("The salon is closed at the requested time") { }
    }

    public class InvalidOperationException : DomainException
    {
        public InvalidOperationException(string message) : base(message) { }
    }
}