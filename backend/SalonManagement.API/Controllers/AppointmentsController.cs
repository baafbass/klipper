//// SalonManagement.API/Controllers/AppointmentsController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SalonManagement.Application.DTOs;
//using SalonManagement.Application.Interfaces;

//namespace SalonManagement.API.Controllers
//{
//    [Authorize]
//    public class AppointmentsController : BaseApiController
//    {
//        private readonly IAppointmentService _appointmentService;

//        public AppointmentsController(IAppointmentService appointmentService)
//        {
//            _appointmentService = appointmentService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
//        {
//            var result = await _appointmentService.CreateAppointmentAsync(dto);
//            return HandleResult(result);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var result = await _appointmentService.GetAppointmentByIdAsync(id);
//            return HandleResult(result);
//        }

//        [HttpGet("customer/{customerId}")]
//        public async Task<IActionResult> GetCustomerAppointments(Guid customerId)
//        {
//            var result = await _appointmentService.GetCustomerAppointmentsAsync(customerId);
//            return HandleResult(result);
//        }

//        [HttpGet("employee/{employeeId}")]
//        [Authorize(Roles = "Employee,SalonManager,SystemAdmin")]
//        public async Task<IActionResult> GetEmployeeAppointments(Guid employeeId, [FromQuery] DateTime date)
//        {
//            var result = await _appointmentService.GetEmployeeAppointmentsAsync(employeeId, date);
//            return HandleResult(result);
//        }

//        [HttpGet("salon/{salonId}")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> GetSalonAppointments(
//            Guid salonId,
//            [FromQuery] DateTime startDate,
//            [FromQuery] DateTime endDate)
//        {
//            var result = await _appointmentService.GetSalonAppointmentsAsync(salonId, startDate, endDate);
//            return HandleResult(result);
//        }

//        [HttpPost("availability")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetAvailableTimeSlots([FromBody] AvailabilityRequestDto request)
//        {
//            var result = await _appointmentService.GetAvailableTimeSlotsAsync(request);
//            return HandleResult(result);
//        }

//        [HttpPatch("{id}/confirm")]
//        [Authorize(Roles = "Employee,SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Confirm(Guid id)
//        {
//            var result = await _appointmentService.ConfirmAppointmentAsync(id);
//            return HandleResult(result);
//        }

//        [HttpPatch("{id}/cancel")]
//        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentDto dto)
//        {
//            var result = await _appointmentService.CancelAppointmentAsync(id, dto.Reason);
//            return HandleResult(result);
//        }

//        [HttpPatch("{id}/complete")]
//        [Authorize(Roles = "Employee,SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Complete(Guid id)
//        {
//            var result = await _appointmentService.CompleteAppointmentAsync(id);
//            return HandleResult(result);
//        }
//    }

//    public class CancelAppointmentDto
//    {
//        public string Reason { get; set; }
//    }
//}