//// SalonManagement.API/Controllers/EmployeesController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SalonManagement.Application.DTOs;
//using SalonManagement.Application.Interfaces;

//namespace SalonManagement.API.Controllers
//{
//    [Authorize]
//    public class EmployeesController : BaseApiController
//    {
//        private readonly IEmployeeService _employeeService;

//        public EmployeesController(IEmployeeService employeeService)
//        {
//            _employeeService = employeeService;
//        }

//        [HttpGet("salon/{salonId}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetBySalon(Guid salonId)
//        {
//            var result = await _employeeService.GetEmployeesBySalonAsync(salonId);
//            return HandleResult(result);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var result = await _employeeService.GetEmployeeByIdAsync(id);
//            return HandleResult(result);
//        }

//        [HttpPost]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
//        {
//            var result = await _employeeService.CreateEmployeeAsync(dto);
//            return HandleResult(result);
//        }

//        [HttpPost("{employeeId}/services/{serviceId}")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> AssignService(Guid employeeId, Guid serviceId)
//        {
//            var result = await _employeeService.AssignServiceToEmployeeAsync(employeeId, serviceId);
//            return HandleResult(result);
//        }

//        [HttpDelete("{employeeId}/services/{serviceId}")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> RemoveService(Guid employeeId, Guid serviceId)
//        {
//            var result = await _employeeService.RemoveServiceFromEmployeeAsync(employeeId, serviceId);
//            return HandleResult(result);
//        }

//        [HttpGet("{employeeId}/schedules")]
//        public async Task<IActionResult> GetSchedules(Guid employeeId)
//        {
//            var result = await _employeeService.GetEmployeeSchedulesAsync(employeeId);
//            return HandleResult(result);
//        }

//        [HttpPost("schedules")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> CreateSchedule([FromBody] CreateEmployeeScheduleDto dto)
//        {
//            var result = await _employeeService.CreateScheduleAsync(dto);
//            return HandleResult(result);
//        }
//    }
//}