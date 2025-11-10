using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.Data; // your ApplicationDbContext namespace
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.DTOs;
using SalonManagement.API.DTOs.Manager;
using SalonManagement.API.Repositories.Interfaces;
using System.Security.Claims;
using ManagerDtos = SalonManagement.API.DTOs.Manager; // <-- alias

namespace SalonManagement.API.Services
{
    public class SalonManagerService : ISalonManagerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalonManagerService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // helper: get current logged-in manager and salon id
        private async Task<SalonManager?> GetCurrentManagerAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var userIdClaim = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;

            if (!Guid.TryParse(userIdClaim, out var uid)) return null;

            return await _context.SalonManagers.FirstOrDefaultAsync(sm => sm.Id == uid, cancellationToken);
        }

        #region WorkingHours

        public async Task<Result<IEnumerable<WorkingHoursDto>>> GetWorkingHoursAsync(CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<IEnumerable<WorkingHoursDto>>("Unauthorized.");

            var wh = await _context.SalonWorkingHours
                .Where(w => w.SalonId == manager.SalonId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<IEnumerable<WorkingHoursDto>>(wh);
            return Result.Success(dtos);
        }

        public async Task<Result<WorkingHoursDto>> AddWorkingHoursAsync(CreateWorkingHoursDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<WorkingHoursDto>("Unauthorized.");

            // prevent duplicate for same day
            var exists = await _context.SalonWorkingHours.AnyAsync(w => w.SalonId == manager.SalonId && w.DayOfWeek == dto.DayOfWeek, cancellationToken);
            if (exists) return Result.Failure<WorkingHoursDto>("Working hours for this day already exist.");

            var entity = new SalonWorkingHours(manager.SalonId, dto.DayOfWeek, dto.OpenTime, dto.CloseTime);
            if (!dto.IsOpen) entity.SetOpen(false);

            await _context.SalonWorkingHours.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var outDto = _mapper.Map<WorkingHoursDto>(entity);
            return Result.Success(outDto);
        }

        public async Task<Result<WorkingHoursDto>> UpdateWorkingHoursAsync(Guid id, UpdateWorkingHoursDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<WorkingHoursDto>("Unauthorized.");

            var entity = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.Id == id && w.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure<WorkingHoursDto>("Working hours not found.");

            entity.UpdateHours(dto.OpenTime, dto.CloseTime);
            entity.SetOpen(dto.IsOpen);

            _context.SalonWorkingHours.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<WorkingHoursDto>(entity));
        }

        public async Task<Result> DeleteWorkingHoursAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var entity = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.Id == id && w.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure("Working hours not found.");

            _context.SalonWorkingHours.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        #endregion

        #region Services

        public async Task<Result<IEnumerable<ServiceDto>>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<IEnumerable<ServiceDto>>("Unauthorized.");

            var services = await _context.Services
                .Where(s => s.SalonId == manager.SalonId && s.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<ServiceDto>>(services));
        }

        public async Task<Result<ServiceDto>> AddServiceAsync(CreateServiceForSalonDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<ServiceDto>("Unauthorized.");

            var entity = new Service(manager.SalonId, dto.Name, dto.Description, dto.DurationMinutes, dto.Price, dto.Category);
            await _context.Services.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<ServiceDto>(entity));
        }

        public async Task<Result<ServiceDto>> UpdateServiceAsync(Guid id, UpdateServiceForSalonDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<ServiceDto>("Unauthorized.");

            var entity = await _context.Services.FirstOrDefaultAsync(s => s.Id == id && s.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure<ServiceDto>("Service not found.");

            entity.UpdateDetails(dto.Name, dto.Description, dto.DurationMinutes, dto.Price, dto.Category);
            _context.Services.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<ServiceDto>(entity));
        }

        public async Task<Result> DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var entity = await _context.Services.FirstOrDefaultAsync(s => s.Id == id && s.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure("Service not found.");

            entity.Deactivate();
            _context.Services.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        #endregion

        #region Employees

        public async Task<Result<IEnumerable<EmployeeDto>>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<IEnumerable<EmployeeDto>>("Unauthorized.");

            var employees = await _context.Employees
                .Where(e => e.SalonId == manager.SalonId && e.IsActive)
                .Include(e => e.EmployeeServices)
                    .ThenInclude(es => es.Service)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Result.Success(dtos);
        }

        public async Task<Result<EmployeeDto>> AddEmployeeAsync(ManagerDtos.CreateEmployeeDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<EmployeeDto>("Unauthorized.");

            // check email uniqueness across user tables (simple)
            var email = dto.Email.Trim().ToLowerInvariant();
            var exists = await _context.SystemAdmins.AnyAsync(u => u.Email.ToLower() == email, cancellationToken)
                         || await _context.SalonManagers.AnyAsync(u => u.Email.ToLower() == email, cancellationToken)
                         || await _context.Customers.AnyAsync(u => u.Email.ToLower() == email, cancellationToken)
                         || await _context.Employees.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);

            if (exists) return Result.Failure<EmployeeDto>("Email already in use.");

            var employee = new Employee(dto.Email, dto.FirstName, dto.LastName, dto.PhoneNumber, manager.SalonId);
            if (dto.CommissionRate.HasValue) employee.SetCommissionRate(dto.CommissionRate.Value);

            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            employee.SetPassword(hashed);

            await _context.Employees.AddAsync(employee, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var outDto = _mapper.Map<EmployeeDto>(employee);
            return Result.Success(outDto);
        }

        public async Task<Result<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<EmployeeDto>("Unauthorized.");

            var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id && e.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure<EmployeeDto>("Employee not found.");

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.PhoneNumber);
            if (dto.CommissionRate.HasValue) entity.SetCommissionRate(dto.CommissionRate.Value);

            _context.Employees.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<EmployeeDto>(entity));
        }

        public async Task<Result> DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id && e.SalonId == manager.SalonId, cancellationToken);
            if (entity == null) return Result.Failure("Employee not found.");

            entity.Deactivate();
            _context.Employees.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        #endregion

        #region EmployeeSchedules

        public async Task<Result<IEnumerable<EmployeeScheduleDto>>> GetEmployeeSchedulesAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<IEnumerable<EmployeeScheduleDto>>("Unauthorized.");

            // verify employee belongs to salon
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (employee == null) return Result.Failure<IEnumerable<EmployeeScheduleDto>>("Employee not found.");

            var schedules = await _context.EmployeeSchedules.Where(s => s.EmployeeId == employeeId && s.IsActive)
                .AsNoTracking().ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<EmployeeScheduleDto>>(schedules));
        }

        public async Task<Result<EmployeeScheduleDto>> AddEmployeeScheduleAsync(ManagerDtos.CreateEmployeeScheduleDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<EmployeeScheduleDto>("Unauthorized.");

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (employee == null) return Result.Failure<EmployeeScheduleDto>("Employee not found.");

            var schedule = new EmployeeSchedule(dto.EmployeeId, dto.DayOfWeek, dto.StartTime, dto.EndTime);
            await _context.EmployeeSchedules.AddAsync(schedule, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<EmployeeScheduleDto>(schedule));
        }

        public async Task<Result<EmployeeScheduleDto>> UpdateEmployeeScheduleAsync(Guid id, UpdateEmployeeScheduleDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure<EmployeeScheduleDto>("Unauthorized.");

            var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (schedule == null) return Result.Failure<EmployeeScheduleDto>("Schedule not found.");

            // verify schedule belongs to salon
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == schedule.EmployeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (employee == null) return Result.Failure<EmployeeScheduleDto>("Not allowed.");

           // schedule.SetActive(dto.IsActive);
            schedule.UpdateSchedule(dto.StartTime, dto.EndTime); // assume method exists or implement UpdateTimes accordingly
            _context.EmployeeSchedules.Update(schedule);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<EmployeeScheduleDto>(schedule));
        }

        public async Task<Result> DeleteEmployeeScheduleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (schedule == null) return Result.Failure("Schedule not found.");

            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.Id == schedule.EmployeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (emp == null) return Result.Failure("Not allowed.");

            schedule.Deactivate(); // assume Deactivate method exists on schedule
            _context.EmployeeSchedules.Update(schedule);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        #endregion

        #region EmployeeServices

        public async Task<Result> AddEmployeeServiceAsync(CreateEmployeeServiceDto dto, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (employee == null) return Result.Failure("Employee not found.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == dto.ServiceId && s.SalonId == manager.SalonId, cancellationToken);
            if (service == null) return Result.Failure("Service not found.");

            var exists = await _context.EmployeeServices.AnyAsync(es => es.EmployeeId == dto.EmployeeId && es.ServiceId == dto.ServiceId, cancellationToken);
            if (exists) return Result.Failure("Employee already assigned this service.");

            var link = new EmployeeService(dto.EmployeeId, dto.ServiceId);
            await _context.EmployeeServices.AddAsync(link, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RemoveEmployeeServiceAsync(Guid employeeServiceId, CancellationToken cancellationToken = default)
        {
            var manager = await GetCurrentManagerAsync(cancellationToken);
            if (manager == null) return Result.Failure("Unauthorized.");

            var link = await _context.EmployeeServices.FirstOrDefaultAsync(es => es.Id == employeeServiceId, cancellationToken);
            if (link == null) return Result.Failure("Link not found.");

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == link.EmployeeId && e.SalonId == manager.SalonId, cancellationToken);
            if (employee == null) return Result.Failure("Not allowed.");

            link.Deactivate();
            _context.EmployeeServices.Update(link);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        #endregion
    }
}
