using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonManagement.API.DTOs.Manager;
using SalonManagement.API.Repositories.Interfaces;
using ManagerDtos = SalonManagement.API.DTOs.Manager; // alias

namespace SalonManagement.API.Controllers
{
    [ApiController]
    [Route("api/manager")]
    [Produces("application/json")]
    [Authorize(Roles = "SalonManager")]
    public class SalonManagerController : BaseApiController
    {
        private readonly ISalonManagerService _service;

        public SalonManagerController(ISalonManagerService service)
        {
            _service = service;
        }

        // Working hours
        [HttpGet("working-hours")]
        public async Task<IActionResult> GetWorkingHours()
            => HandleResult(await _service.GetWorkingHoursAsync());

        [HttpPost("working-hours")]
        public async Task<IActionResult> AddWorkingHours([FromBody] CreateWorkingHoursDto dto)
            => HandleResult(await _service.AddWorkingHoursAsync(dto));

        [HttpPut("working-hours/{id:guid}")]
        public async Task<IActionResult> UpdateWorkingHours(Guid id, [FromBody] UpdateWorkingHoursDto dto)
            => HandleResult(await _service.UpdateWorkingHoursAsync(id, dto));

        [HttpDelete("working-hours/{id:guid}")]
        public async Task<IActionResult> DeleteWorkingHours(Guid id)
            => HandleResult(await _service.DeleteWorkingHoursAsync(id));

        // Services
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
            => HandleResult(await _service.GetServicesAsync());

        [HttpPost("services")]
        public async Task<IActionResult> AddService([FromBody] CreateServiceForSalonDto dto)
            => HandleResult(await _service.AddServiceAsync(dto));

        [HttpPut("services/{id:guid}")]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceForSalonDto dto)
            => HandleResult(await _service.UpdateServiceAsync(id, dto));

        [HttpDelete("services/{id:guid}")]
        public async Task<IActionResult> DeleteService(Guid id)
            => HandleResult(await _service.DeleteServiceAsync(id));

        // Employees
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
            => HandleResult(await _service.GetEmployeesAsync());

        [HttpPost("employees")]
        public async Task<IActionResult> AddEmployee([FromBody] ManagerDtos.CreateEmployeeDto dto)
            => HandleResult(await _service.AddEmployeeAsync(dto));

        [HttpPut("employees/{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
            => HandleResult(await _service.UpdateEmployeeAsync(id, dto));

        [HttpDelete("employees/{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
            => HandleResult(await _service.DeleteEmployeeAsync(id));

        // Employee schedules
        [HttpGet("employees/{employeeId:guid}/schedules")]
        public async Task<IActionResult> GetEmployeeSchedules(Guid employeeId)
            => HandleResult(await _service.GetEmployeeSchedulesAsync(employeeId));

        [HttpPost("employees/schedules")]
        public async Task<IActionResult> AddEmployeeSchedule([FromBody] ManagerDtos.CreateEmployeeScheduleDto dto)
            => HandleResult(await _service.AddEmployeeScheduleAsync(dto));

        [HttpPut("employees/schedules/{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeSchedule(Guid id, [FromBody] UpdateEmployeeScheduleDto dto)
            => HandleResult(await _service.UpdateEmployeeScheduleAsync(id, dto));

        [HttpDelete("employees/schedules/{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeSchedule(Guid id)
            => HandleResult(await _service.DeleteEmployeeScheduleAsync(id));

        // Employee services
        [HttpPost("employees/services")]
        public async Task<IActionResult> AddEmployeeService([FromBody] ManagerDtos.CreateEmployeeServiceDto dto)
            => HandleResult(await _service.AddEmployeeServiceAsync(dto));

        [HttpDelete("employees/services/{id:guid}")]
        public async Task<IActionResult> RemoveEmployeeService(Guid id)
            => HandleResult(await _service.RemoveEmployeeServiceAsync(id));
    }
}