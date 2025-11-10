using SalonManagement.API.Domain.Common;
using SalonManagement.API.DTOs; // existing DTOs
using ManagerDtos = SalonManagement.API.DTOs.Manager; // <-- alias

namespace SalonManagement.API.Repositories.Interfaces
{
    public interface ISalonManagerService
    {
        // Working hours
        Task<Result<IEnumerable<WorkingHoursDto>>> GetWorkingHoursAsync(CancellationToken cancellationToken = default);
        Task<Result<WorkingHoursDto>> AddWorkingHoursAsync(ManagerDtos.CreateWorkingHoursDto dto, CancellationToken cancellationToken = default);
        Task<Result<WorkingHoursDto>> UpdateWorkingHoursAsync(Guid id, ManagerDtos.UpdateWorkingHoursDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteWorkingHoursAsync(Guid id, CancellationToken cancellationToken = default);

        // Services
        Task<Result<IEnumerable<ServiceDto>>> GetServicesAsync(CancellationToken cancellationToken = default);
        Task<Result<ServiceDto>> AddServiceAsync(ManagerDtos.CreateServiceForSalonDto dto, CancellationToken cancellationToken = default);
        Task<Result<ServiceDto>> UpdateServiceAsync(Guid id, ManagerDtos.UpdateServiceForSalonDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default);

        // Employees
        Task<Result<IEnumerable<EmployeeDto>>> GetEmployeesAsync(CancellationToken cancellationToken = default);
        Task<Result<EmployeeDto>> AddEmployeeAsync(ManagerDtos.CreateEmployeeDto dto, CancellationToken cancellationToken = default); // <- alias used
        Task<Result<EmployeeDto>> UpdateEmployeeAsync(Guid id, ManagerDtos.UpdateEmployeeDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken = default);

        // Employee schedule
        Task<Result<IEnumerable<EmployeeScheduleDto>>> GetEmployeeSchedulesAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<Result<EmployeeScheduleDto>> AddEmployeeScheduleAsync(ManagerDtos.CreateEmployeeScheduleDto dto, CancellationToken cancellationToken = default); // <- alias used
        Task<Result<EmployeeScheduleDto>> UpdateEmployeeScheduleAsync(Guid id, ManagerDtos.UpdateEmployeeScheduleDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteEmployeeScheduleAsync(Guid id, CancellationToken cancellationToken = default);

        // Employee service link
        Task<Result> AddEmployeeServiceAsync(ManagerDtos.CreateEmployeeServiceDto dto, CancellationToken cancellationToken = default);
        Task<Result> RemoveEmployeeServiceAsync(Guid employeeServiceId, CancellationToken cancellationToken = default);
    }
}

