using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Data;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.DTOs;
using EmployeeDtos = SalonManagement.API.DTOs.Employee;
using SalonManagement.API.Repositories.Interfaces;
using System.Security.Claims;
using EmployeeServiceEntity = SalonManagement.API.Domain.Entities.EmployeeService;

namespace SalonManagement.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<Employee?> GetCurrentEmployeeAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;
            var userIdClaim = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;
            if (!Guid.TryParse(userIdClaim, out var uid)) return null;

            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == uid, cancellationToken);
        }

        public async Task<Result<IEnumerable<WorkingHoursDto>>> GetSalonWorkingHoursAsync(CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure<IEnumerable<WorkingHoursDto>>("Unauthorized.");

            var wh = await _context.SalonWorkingHours
                .Where(w => w.SalonId == emp.SalonId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<WorkingHoursDto>>(wh));
        }

        public async Task<Result<IEnumerable<EmployeeScheduleDto>>> GetMySchedulesAsync(CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure<IEnumerable<EmployeeScheduleDto>>("Unauthorized.");

            var schedules = await _context.EmployeeSchedules
                .Where(s => s.EmployeeId == emp.Id && s.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<EmployeeScheduleDto>>(schedules));
        }

        public async Task<Result<EmployeeScheduleDto>> AddMyScheduleAsync(EmployeeDtos.EmployeeScheduleRequestDto dto, CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure<EmployeeScheduleDto>("Unauthorized.");

            // ensure schedule falls within salon working hours for that day (if salon is closed or manager set closed, reject)
            var salonWh = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.SalonId == emp.SalonId && w.DayOfWeek == dto.DayOfWeek, cancellationToken);
            if (salonWh == null || !salonWh.IsOpen)
                return Result.Failure<EmployeeScheduleDto>("Salon is closed on that day; cannot add schedule.");

            if (dto.StartTime < salonWh.OpenTime || dto.EndTime > salonWh.CloseTime)
                return Result.Failure<EmployeeScheduleDto>("Schedule must be within salon working hours.");

            if (dto.EndTime <= dto.StartTime)
                return Result.Failure<EmployeeScheduleDto>("End time must be after start time.");

            // optional: prevent overlapping schedules for employee same day
            var overlaps = await _context.EmployeeSchedules.AnyAsync(s =>
                s.EmployeeId == emp.Id && s.DayOfWeek == dto.DayOfWeek &&
                s.IsActive &&
                !(dto.EndTime <= s.StartTime || dto.StartTime >= s.EndTime), cancellationToken);
            if (overlaps) return Result.Failure<EmployeeScheduleDto>("Schedule overlaps an existing one.");

            var schedule = new EmployeeSchedule(emp.Id, dto.DayOfWeek, dto.StartTime, dto.EndTime);
            await _context.EmployeeSchedules.AddAsync(schedule, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<EmployeeScheduleDto>(schedule));
        }

        //public async Task<Result<EmployeeScheduleDto>> UpdateMyScheduleAsync(Guid id, EmployeeDtos.EmployeeScheduleRequestDto dto, CancellationToken cancellationToken = default)
        //{
        //    var emp = await GetCurrentEmployeeAsync(cancellationToken);
        //    if (emp == null) return Result.Failure<EmployeeScheduleDto>("Unauthorized.");

        //    var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(s => s.Id == id && s.EmployeeId == emp.Id, cancellationToken);
        //    if (schedule == null) return Result.Failure<EmployeeScheduleDto>("Schedule not found.");

        //    // validate against salon working hours
        //    var salonWh = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.SalonId == emp.SalonId && w.DayOfWeek == dto.DayOfWeek, cancellationToken);
        //    if (salonWh == null || !salonWh.IsOpen)
        //        return Result.Failure<EmployeeScheduleDto>("Salon is closed on that day; cannot update schedule.");
        //    if (dto.StartTime < salonWh.OpenTime || dto.EndTime > salonWh.CloseTime)
        //        return Result.Failure<EmployeeScheduleDto>("Schedule must be within salon working hours.");
        //    if (dto.EndTime <= dto.StartTime)
        //        return Result.Failure<EmployeeScheduleDto>("End time must be after start time.");

        //    // optional: check overlap excluding this schedule
        //    var overlaps = await _context.EmployeeSchedules.AnyAsync(s =>
        //        s.EmployeeId == emp.Id && s.DayOfWeek == dto.DayOfWeek && s.Id != id &&
        //        s.IsActive &&
        //        !(dto.EndTime <= s.StartTime || dto.StartTime >= s.EndTime), cancellationToken);
        //    if (overlaps) return Result.Failure<EmployeeScheduleDto>("Schedule overlaps an existing one.");

        //    schedule.UpdateSchedule(dto.StartTime, dto.EndTime);
        //    _context.EmployeeSchedules.Update(schedule);
        //    await _context.SaveChangesAsync(cancellationToken);

        //    return Result.Success(_mapper.Map<EmployeeScheduleDto>(schedule));
        //}

        public async Task<Result> DeleteMyScheduleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure("Unauthorized.");

            var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(s => s.Id == id && s.EmployeeId == emp.Id, cancellationToken);
            if (schedule == null) return Result.Failure("Schedule not found.");

            schedule.Deactivate();
            _context.EmployeeSchedules.Update(schedule);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<ServiceDto>>> GetSalonServicesAsync(CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure<IEnumerable<ServiceDto>>("Unauthorized.");

            var services = await _context.Services.Where(s => s.SalonId == emp.SalonId && s.IsActive).AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success(_mapper.Map<IEnumerable<ServiceDto>>(services));
        }

        public async Task<Result<IEnumerable<ServiceDto>>> GetMyServicesAsync(CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure<IEnumerable<ServiceDto>>("Unauthorized.");

            var services = await _context.EmployeeServices
                .Where(es => es.EmployeeId == emp.Id && es.IsActive)
                .Include(es => es.Service)
                .Select(es => es.Service)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<ServiceDto>>(services));
        }

        public async Task<Result> AssignServiceToMeAsync(EmployeeDtos.EmployeeServiceAssignDto dto, CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure("Unauthorized.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == dto.ServiceId && s.SalonId == emp.SalonId && s.IsActive, cancellationToken);
            if (service == null) return Result.Failure("Service not found.");

            var exists = await _context.EmployeeServices.AnyAsync(es => es.EmployeeId == emp.Id && es.ServiceId == dto.ServiceId && es.IsActive, cancellationToken);
            if (exists) return Result.Failure("Service already assigned.");

            var link = new EmployeeServiceEntity(emp.Id, dto.ServiceId);
            await _context.EmployeeServices.AddAsync(link, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> RemoveMyServiceAsync(Guid employeeServiceId, CancellationToken cancellationToken = default)
        {
            var emp = await GetCurrentEmployeeAsync(cancellationToken);
            if (emp == null) return Result.Failure("Unauthorized.");

            var link = await _context.EmployeeServices.FirstOrDefaultAsync(es => es.ServiceId == employeeServiceId && es.EmployeeId == emp.Id, cancellationToken);
            if (link == null) return Result.Failure("Link not found.");

            link.Deactivate();
            _context.EmployeeServices.Update(link);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}