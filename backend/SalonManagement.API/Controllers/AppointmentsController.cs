using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonManagement.API.DTOs;
using SalonManagement.API.Repositories.Interfaces;

namespace SalonManagement.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Produces("application/json")]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        // Customer: get my appointments
        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyAppointments()
            => HandleResult(await _service.GetMyAppointmentsAsync());

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
            => HandleResult(await _service.GetAppointmentByIdAsync(id));

        // Availability (anyone authenticated or anonymous if you prefer)
        [HttpPost("availability")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailability([FromBody] AvailabilityRequestDto request)
            => HandleResult(await _service.GetAvailabilityAsync(request));

        // Create appointment (customer)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
            => HandleResult(await _service.CreateAppointmentAsync(dto));

        // Confirm (manager or employee)
        [HttpPost("{id:guid}/confirm")]
        [Authorize(Roles = "SalonManager,Employee")]
        public async Task<IActionResult> Confirm(Guid id)
            => HandleResult(await _service.ConfirmAppointmentAsync(id));

        // Cancel
        [HttpPost("{id:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequestDto body)
            => HandleResult(await _service.CancelAppointmentAsync(id, body.Reason));
    }

    public class CancelAppointmentRequestDto
    {
        public string Reason { get; set; }
    }
}