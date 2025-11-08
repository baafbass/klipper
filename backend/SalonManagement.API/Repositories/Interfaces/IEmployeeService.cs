using SalonManagement.API.DTOs;
using SalonManagement.API.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SalonManagement.API.Repositories.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<IEnumerable<EmployeeDto>>> GetEmployeesBySalonAsync(Guid salonId, CancellationToken cancellationToken = default);
        Task<Result<EmployeeDto>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);
        Task<Result> AssignServiceToEmployeeAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default);
        Task<Result> RemoveServiceFromEmployeeAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default);
        Task<Result<EmployeeScheduleDto>> CreateScheduleAsync(CreateEmployeeScheduleDto dto, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<EmployeeScheduleDto>>> GetEmployeeSchedulesAsync(Guid employeeId, CancellationToken cancellationToken = default);
    }

    public class CreateEmployeeDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Guid SalonId { get; set; }
        public decimal? CommissionRate { get; set; }
    }
}