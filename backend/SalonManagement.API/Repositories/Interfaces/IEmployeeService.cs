using SalonManagement.API.Domain.Common;
using SalonManagement.API.DTOs;
using ManagerDtos = SalonManagement.API.DTOs.Manager;
using EmployeeDtos = SalonManagement.API.DTOs.Employee;

namespace SalonManagement.API.Repositories.Interfaces
{
    public interface IEmployeeService
    {
        // Salon working hours for the employee's salon (read-only)
        Task<Result<IEnumerable<WorkingHoursDto>>> GetSalonWorkingHoursAsync(CancellationToken cancellationToken = default);

        // Employee schedules
        Task<Result<IEnumerable<EmployeeScheduleDto>>> GetMySchedulesAsync(CancellationToken cancellationToken = default);
        Task<Result<EmployeeScheduleDto>> AddMyScheduleAsync(EmployeeDtos.EmployeeScheduleRequestDto dto, CancellationToken cancellationToken = default);
        Task<Result<EmployeeScheduleDto>> UpdateMyScheduleAsync(Guid id, EmployeeDtos.EmployeeScheduleRequestDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteMyScheduleAsync(Guid id, CancellationToken cancellationToken = default);

        // Services - available services in the salon + employee's assigned services
        Task<Result<IEnumerable<ServiceDto>>> GetSalonServicesAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ServiceDto>>> GetMyServicesAsync(CancellationToken cancellationToken = default);
        Task<Result> AssignServiceToMeAsync(EmployeeDtos.EmployeeServiceAssignDto dto, CancellationToken cancellationToken = default);
        Task<Result> RemoveMyServiceAsync(Guid employeeServiceId, CancellationToken cancellationToken = default);
    }
}





//using SalonManagement.API.DTOs;
//using SalonManagement.API.Domain.Common;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SalonManagement.API.Repositories.Interfaces
//{
//    public interface IEmployeeService
//    {
//        Task<Result<IEnumerable<EmployeeDto>>> GetEmployeesBySalonAsync(Guid salonId, CancellationToken cancellationToken = default);
//        Task<Result<EmployeeDto>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default);
//        Task<Result<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);
//        Task<Result> AssignServiceToEmployeeAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default);
//        Task<Result> RemoveServiceFromEmployeeAsync(Guid employeeId, Guid serviceId, CancellationToken cancellationToken = default);
//        Task<Result<EmployeeScheduleDto>> CreateScheduleAsync(CreateEmployeeScheduleDto dto, CancellationToken cancellationToken = default);
//        Task<Result<IEnumerable<EmployeeScheduleDto>>> GetEmployeeSchedulesAsync(Guid employeeId, CancellationToken cancellationToken = default);
//    }

//    public class CreateEmployeeDto
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string PhoneNumber { get; set; }
//        public Guid SalonId { get; set; }
//        public decimal? CommissionRate { get; set; }
//    }
//}