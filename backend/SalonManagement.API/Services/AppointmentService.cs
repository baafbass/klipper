using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalonManagement.API.Data;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.DTOs;
using SalonManagement.API.Repositories.Interfaces;
using System.Security.Claims;

namespace SalonManagement.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<AppointmentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<Customer?> GetCurrentCustomerAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;
            var userIdClaim = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;
            if (!Guid.TryParse(userIdClaim, out var uid)) return null;
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == uid, cancellationToken);
        }

        public async Task<Result<IEnumerable<AppointmentDto>>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default)
        {
            var cust = await GetCurrentCustomerAsync(cancellationToken);
            if (cust == null) return Result.Failure<IEnumerable<AppointmentDto>>("Unauthorized.");

            var appointments = await _context.Appointments
                .Where(a => a.CustomerId == cust.Id)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
        }

        public async Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var appt = await _context.Appointments
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.Employee).Include(a => a.Salon).Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (appt == null) return Result.Failure<AppointmentDto>("Not found.");

            return Result.Success(_mapper.Map<AppointmentDto>(appt));
        }

        public async Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailabilityAsync(AvailabilityRequestDto request, CancellationToken cancellationToken = default)
        {
            // load candidate employees (either a specific one or all employees of salon who can perform all selected services)
            var employeesQuery = _context.Employees.Where(e => e.SalonId == request.SalonId && e.IsActive);

            if (request.EmployeeId.HasValue)
                employeesQuery = employeesQuery.Where(e => e.Id == request.EmployeeId.Value);

            var employees = await employeesQuery
                .Include(e => e.EmployeeServices)
                    .ThenInclude(es => es.Service)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!employees.Any()) return Result.Success(Enumerable.Empty<AvailableTimeSlotDto>());

            // compute total duration and total price of chosen services (using service records)
            var services = await _context.Services.Where(s => request.ServiceIds.Contains(s.Id) && s.IsActive && s.SalonId == request.SalonId)
                .AsNoTracking().ToListAsync(cancellationToken);

            if (services.Count != request.ServiceIds.Count)
                return Result.Failure<IEnumerable<AvailableTimeSlotDto>>("One or more services not available.");

            var totalDuration = services.Sum(s => s.DurationMinutes);
            var totalPrice = services.Sum(s => s.Price);

            var date = request.Date.Date;

            var results = new List<AvailableTimeSlotDto>();

            // For each employee, compute available slots by intersecting salon working hours, employee schedules, and existing appointments
            foreach (var emp in employees)
            {
                // check employee can perform all services
                var empServiceIds = emp.EmployeeServices.Where(es => es.IsActive).Select(es => es.ServiceId).ToHashSet();
                if (!request.ServiceIds.All(id => empServiceIds.Contains(id)))
                    continue; // skip this employee

                // salon working hours for this day
                var salonWh = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.SalonId == request.SalonId && w.DayOfWeek == (int)date.DayOfWeek, cancellationToken);
                if (salonWh == null || !salonWh.IsOpen) continue;

                // employee schedules for this day
                var sched = await _context.EmployeeSchedules
                    .Where(s => s.EmployeeId == emp.Id && s.DayOfWeek == (int)date.DayOfWeek && s.IsActive)
                    .AsNoTracking().ToListAsync(cancellationToken);
                if (!sched.Any()) continue;

                // existing appointments for employee that day
                var appts = await _context.Appointments
                    .Where(a => a.EmployeeId == emp.Id && a.AppointmentDate == date && a.Status != AppointmentStatus.Cancelled)
                    .AsNoTracking().ToListAsync(cancellationToken);

                // For each employee schedule block, produce candidate start times with granularity (15 minutes)
                foreach (var block in sched)
                {
                    // intersect schedule with salon hours
                    var start = block.StartTime < salonWh.OpenTime ? salonWh.OpenTime : block.StartTime;
                    var end = block.EndTime > salonWh.CloseTime ? salonWh.CloseTime : block.EndTime;

                    for (var candidate = start; candidate + TimeSpan.FromMinutes(totalDuration) <= end; candidate = candidate.Add(TimeSpan.FromMinutes(15)))
                    {
                        var candidateEnd = candidate.Add(TimeSpan.FromMinutes(totalDuration));

                        // check appointment conflicts
                        var hasConflict = appts.Any(a => !(a.EndTime <= candidate || a.StartTime >= candidateEnd));
                        if (hasConflict) continue;

                        // additionally ensure no overlapping with non-working (should be fine)
                        results.Add(new AvailableTimeSlotDto
                        {
                            StartTime = candidate,
                            EndTime = candidateEnd,
                            EmployeeId = emp.Id,
                            EmployeeName = $"{emp.FirstName} {emp.LastName}"
                        });
                    }
                }
            }

            // Optionally sort results by start time and employee name
            var ordered = results.OrderBy(r => r.StartTime).ThenBy(r => r.EmployeeName).ToList();
            return Result.Success<IEnumerable<AvailableTimeSlotDto>>(ordered);
        }

        public async Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Customer must be logged in (or we could allow guest)
                var cust = await GetCurrentCustomerAsync(cancellationToken);
                if (cust == null) return Result.Failure<AppointmentDto>("Unauthorized.");

                if (cust.Id != dto.CustomerId) return Result.Failure<AppointmentDto>("Customer mismatch.");

                // Validate salon & employee exist & belong
                var salon = await _context.Salons.FirstOrDefaultAsync(s => s.Id == dto.SalonId, cancellationToken);
                if (salon == null) return Result.Failure<AppointmentDto>("Salon not found.");

                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId && e.SalonId == dto.SalonId, cancellationToken);
                if (employee == null) return Result.Failure<AppointmentDto>("Employee not found or not in salon.");

                // load services and validate
                var services = await _context.Services
                    .Where(s => dto.ServiceIds.Contains(s.Id) && s.IsActive && s.SalonId == dto.SalonId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (services.Count != dto.ServiceIds.Count)
                    return Result.Failure<AppointmentDto>("One or more services invalid or not available.");

                var totalDuration = services.Sum(s => s.DurationMinutes);
                var totalPrice = services.Sum(s => s.Price);

                // check salon working hours for that day
                var date = dto.AppointmentDate.Date;
                var salonWh = await _context.SalonWorkingHours.FirstOrDefaultAsync(w => w.SalonId == dto.SalonId && w.DayOfWeek == (int)date.DayOfWeek, cancellationToken);
                if (salonWh == null || !salonWh.IsOpen) return Result.Failure<AppointmentDto>("Salon closed on selected day.");

                var startTime = dto.StartTime;
                var endTime = startTime.Add(TimeSpan.FromMinutes(totalDuration));
                if (startTime < salonWh.OpenTime || endTime > salonWh.CloseTime) return Result.Failure<AppointmentDto>("Requested time is outside salon working hours.");

                // check employee schedule availability
                var empScheduleOk = await _context.EmployeeSchedules.AnyAsync(s =>
                    s.EmployeeId == employee.Id && s.DayOfWeek == (int)date.DayOfWeek && s.IsActive &&
                    s.StartTime <= startTime && s.EndTime >= endTime, cancellationToken);

                if (!empScheduleOk) return Result.Failure<AppointmentDto>("Employee not scheduled/available at requested time.");

                // check employee assigned to those services
                var empServiceIds = await _context.EmployeeServices
                    .Where(es => es.EmployeeId == employee.Id && es.IsActive)
                    .Select(es => es.ServiceId)
                    .ToListAsync(cancellationToken);

                if (!dto.ServiceIds.All(id => empServiceIds.Contains(id))) return Result.Failure<AppointmentDto>("Employee cannot perform one or more selected services.");

                // check for appointment conflicts
                var conflict = await _context.Appointments.AnyAsync(a =>
                    a.EmployeeId == employee.Id && a.AppointmentDate == date &&
                    !(a.EndTime <= startTime || a.StartTime >= endTime) &&
                    a.Status != AppointmentStatus.Cancelled, cancellationToken);

                if (conflict) return Result.Failure<AppointmentDto>("Requested time conflicts with existing appointment.");

                // create appointment aggregate root
                var appointment = new Appointment(dto.CustomerId, dto.EmployeeId, dto.SalonId, date, startTime);

                // add services to appointment (recalculates totals & end time)
                foreach (var s in services)
                {
                    appointment.AddService(s);
                }

                // optional: defensive check to ensure totals are consistent
                if (appointment.TotalDurationMinutes != totalDuration)
                {
                    _logger.LogWarning("Appointment total duration mismatch computed ({Computed}) vs sum({Sum})", appointment.TotalDurationMinutes, totalDuration);
                }

                // persist
                await _context.Appointments.AddAsync(appointment, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var outDto = _mapper.Map<AppointmentDto>(appointment);
                return Result.Success(outDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed creating appointment for customer {CustomerId}, salon {SalonId}", dto?.CustomerId, dto?.SalonId);
                return Result.Failure<AppointmentDto>("Failed to create appointment: " + ex.Message);
            }
        }


        public async Task<Result> ConfirmAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            // Who can confirm? For simplicity allow salon manager or employee assigned — check claims
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return Result.Failure("Unauthorized.");

            var claimRole = user.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var uid)) return Result.Failure("Unauthorized.");

            var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
            if (appt == null) return Result.Failure("Appointment not found.");

            // manager of the salon or employee who owns the appointment can confirm
            var isManager = claimRole == UserRole.SalonManager.ToString() && await _context.SalonManagers.AnyAsync(sm => sm.Id == uid && sm.SalonId == appt.SalonId, cancellationToken);
            var isEmployee = claimRole == UserRole.Employee.ToString() && appt.EmployeeId == uid;

            if (!isManager && !isEmployee) return Result.Failure("Not allowed to confirm.");

            appt.Confirm();
            _context.Appointments.Update(appt);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> CancelAppointmentAsync(Guid appointmentId, string reason, CancellationToken cancellationToken = default)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return Result.Failure("Unauthorized.");
            var userIdClaim = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var uid)) return Result.Failure("Unauthorized.");

            var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
            if (appt == null) return Result.Failure("Appointment not found.");

            // allow customer who created, salon manager, or employee assigned to cancel
            var claimRole = user.FindFirst(ClaimTypes.Role)?.Value;
            var isCustomer = claimRole == UserRole.Customer.ToString() && appt.CustomerId == uid;
            var isManager = claimRole == UserRole.SalonManager.ToString() && await _context.SalonManagers.AnyAsync(sm => sm.Id == uid && sm.SalonId == appt.SalonId, cancellationToken);
            var isEmployee = claimRole == UserRole.Employee.ToString() && appt.EmployeeId == uid;

            if (!isCustomer && !isManager && !isEmployee) return Result.Failure("Not allowed to cancel.");

            appt.Cancel(reason);
            _context.Appointments.Update(appt);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}