//// SalonManagement.API/Controllers/ServicesController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SalonManagement.Application.DTOs;
//using SalonManagement.Application.Interfaces;

//namespace SalonManagement.API.Controllers
//{
//    public class ServicesController : BaseApiController
//    {
//        private readonly IServiceService _serviceService;

//        public ServicesController(IServiceService serviceService)
//        {
//            _serviceService = serviceService;
//        }

//        [HttpGet("salon/{salonId}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetBySalon(Guid salonId)
//        {
//            var result = await _serviceService.GetServicesBySalonAsync(salonId);
//            return HandleResult(result);
//        }

//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var result = await _serviceService.GetServiceByIdAsync(id);
//            return HandleResult(result);
//        }

//        [HttpPost]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
//        {
//            var result = await _serviceService.CreateServiceAsync(dto);
//            return HandleResult(result);
//        }

//        [HttpPut("{id}")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceDto dto)
//        {
//            var result = await _serviceService.UpdateServiceAsync(id, dto);
//            return HandleResult(result);
//        }

//        [HttpDelete("{id}")]
//        [Authorize(Roles = "SalonManager,SystemAdmin")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var result = await _serviceService.DeleteServiceAsync(id);
//            return HandleResult(result);
//        }
//    }
//}