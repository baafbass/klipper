using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeDtos = SalonManagement.API.DTOs.Employee;
using SalonManagement.API.Repositories.Interfaces;

namespace SalonManagement.API.Controllers
{
    [ApiController]
    [Route("api/employee")]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : BaseApiController
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service) => _service = service;

        [HttpGet("working-hours")]
        public async Task<IActionResult> GetSalonWorkingHours()
            => HandleResult(await _service.GetSalonWorkingHoursAsync());

        [HttpGet("schedules")]
        public async Task<IActionResult> GetMySchedules()
            => HandleResult(await _service.GetMySchedulesAsync());

        [HttpPost("schedules")]
        public async Task<IActionResult> AddMySchedule([FromBody] EmployeeDtos.EmployeeScheduleRequestDto dto)
            => HandleResult(await _service.AddMyScheduleAsync(dto));

        [HttpPut("schedules/{id:guid}")]
        public async Task<IActionResult> UpdateMySchedule(Guid id, [FromBody] EmployeeDtos.EmployeeScheduleRequestDto dto)
            => HandleResult(await _service.UpdateMyScheduleAsync(id, dto));

        [HttpDelete("schedules/{id:guid}")]
        public async Task<IActionResult> DeleteMySchedule(Guid id)
            => HandleResult(await _service.DeleteMyScheduleAsync(id));

        [HttpGet("services")]
        public async Task<IActionResult> GetSalonServices() => HandleResult(await _service.GetSalonServicesAsync());

        [HttpGet("my-services")]
        public async Task<IActionResult> GetMyServices() => HandleResult(await _service.GetMyServicesAsync());

        [HttpPost("my-services")]
        public async Task<IActionResult> AssignService([FromBody] EmployeeDtos.EmployeeServiceAssignDto dto)
            => HandleResult(await _service.AssignServiceToMeAsync(dto));

        [HttpDelete("my-services/{id:guid}")]
        public async Task<IActionResult> RemoveMyService(Guid id)
            => HandleResult(await _service.RemoveMyServiceAsync(id));
    }
}
