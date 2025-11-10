// SalonManagement.API/Controllers/SalonsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonManagement.API.DTOs;
using SalonManagement.API.Repositories.Interfaces;

namespace SalonManagement.API.Controllers
{
    public class SalonsController : BaseApiController
    {
        private readonly ISalonService _salonService;

        public SalonsController(ISalonService salonService)
        {
            _salonService = salonService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _salonService.GetAllSalonsAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _salonService.GetSalonByIdAsync(id);
            return HandleResult(result);
        }

        //[HttpGet("search")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SearchByCity([FromQuery] string city)
        //{
        //    var result = await _salonService.SearchSalonsByCityAsync(city);
        //    return HandleResult(result);
        //}

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateSalonDto dto)
        {
            var result = await _salonService.CreateSalonAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SalonManager,SystemAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalonDto dto)
        {
            var result = await _salonService.UpdateSalonAsync(id, dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _salonService.DeleteSalonAsync(id);
            return HandleResult(result);
        }

        [HttpPost("{id}/manager")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> AddManager(Guid id, [FromBody] SalonManagerRequestDto dto)
        {
            var result = await _salonService.AddManagerAsync(id, dto);
            return HandleResult(result);
        }

    }
}