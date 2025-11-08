using AutoMapper;
using SalonManagement.API.DTOs;
using SalonManagement.API.Domain.Interfaces;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Repositories.Interfaces;

namespace SalonManagement.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<AppointmentDto>> CreateAppointmentAsync(
            CreateAppointmentDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate salon exists and is open
                var salon = await _unitOfWork.Salons.GetByIdAsync(dto.SalonId, cancellationToken);
                if (salon == null)
                    return Result.Failure<AppointmentDto>("Salon not found");

                var appointmentDateTime = dto.AppointmentDate.Add(dto.StartTime);
                if (!salon.IsOpenAt(appointmentDateTime))
                    return Result.Failure<AppointmentDto>("Salon is closed at the requested time");

                // Validate employee
                var employee = await _unitOfWork.Employees.GetEmployeeWithSchedulesAsync(
                    dto.EmployeeId, cancellationToken);
                if (employee == null)
                    return Result.Failure<AppointmentDto>("Employee not found");

                // Validate customer
                var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId, cancellationToken);
                if (customer == null)
                    return Result.Failure<AppointmentDto>("Customer not found");

                // Get services
                var services = new List<Service>();
                foreach (var serviceId in dto.ServiceIds)
                {
                    var service = await _unitOfWork.Services.GetByIdAsync(serviceId, cancellationToken);
                    if (service == null || !service.IsActive)
                        return Result.Failure<AppointmentDto>($"Service {serviceId} not found or inactive");

                    services.Add(service);
                }

                // Calculate total duration
                var totalDuration = services.Sum(s => s.DurationMinutes);
                var endTime = dto.StartTime.Add(TimeSpan.FromMinutes(totalDuration));

                // Check employee availability
                if (!employee.IsAvailable(appointmentDateTime, appointmentDateTime.Add(TimeSpan.FromMinutes(totalDuration))))
                    return Result.Failure<AppointmentDto>("Employee is not available at the requested time");

                // Check for conflicts
                var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                    dto.EmployeeId, dto.AppointmentDate, dto.StartTime, endTime, null, cancellationToken);

                if (hasConflict)
                    return Result.Failure<AppointmentDto>("The requested time slot conflicts with an existing appointment");

                // Create appointment
                var appointment = new Appointment(
                    dto.CustomerId, dto.EmployeeId, dto.SalonId,
                    dto.AppointmentDate, dto.StartTime);

                appointment.UpdateNotes(dto.Notes);

                foreach (var service in services)
                {
                    appointment.AddService(service);
                }

                await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var result = await GetAppointmentByIdAsync(appointment.Id, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<AppointmentDto>($"Error creating appointment: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(
            AvailabilityRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Get services to calculate duration
                var services = new List<Service>();
                foreach (var serviceId in request.ServiceIds)
                {
                    var service = await _unitOfWork.Services.GetByIdAsync(serviceId, cancellationToken);
                    if (service != null) services.Add(service);
                }

                var totalDuration = services.Sum(s => s.DurationMinutes);

                // Get employees
                IEnumerable<Employee> employees;
                if (request.EmployeeId.HasValue)
                {
                    var emp = await _unitOfWork.Employees.GetEmployeeWithSchedulesAsync(
                        request.EmployeeId.Value, cancellationToken);
                    employees = emp != null ? new[] { emp } : Array.Empty<Employee>();
                }
                else
                {
                    employees = await _unitOfWork.Employees.GetBySalonIdAsync(
                        request.SalonId, cancellationToken);
                }

                var availableSlots = new List<AvailableTimeSlotDto>();
                var dayOfWeek = (int)request.Date.DayOfWeek;

                foreach (var employee in employees)
                {
                    var schedules = employee.Schedules
                        .Where(s => s.DayOfWeek == dayOfWeek && s.IsActive)
                        .OrderBy(s => s.StartTime);

                    foreach (var schedule in schedules)
                    {
                        var currentTime = schedule.StartTime;
                        var endLimit = schedule.EndTime.Add(TimeSpan.FromMinutes(-totalDuration));

                        while (currentTime <= endLimit)
                        {
                            var slotEnd = currentTime.Add(TimeSpan.FromMinutes(totalDuration));

                            var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                                employee.Id, request.Date, currentTime, slotEnd, null, cancellationToken);

                            if (!hasConflict)
                            {
                                availableSlots.Add(new AvailableTimeSlotDto
                                {
                                    StartTime = currentTime,
                                    EndTime = slotEnd,
                                    EmployeeId = employee.Id,
                                    EmployeeName = employee.GetFullName()
                                });
                            }

                            currentTime = currentTime.Add(TimeSpan.FromMinutes(30)); // 30-minute intervals
                        }
                    }
                }

                return Result.Success(availableSlots.AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<AvailableTimeSlotDto>>($"Error getting available slots: {ex.Message}");
            }
        }

        // Implement other methods...

        public async Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try to retrieve with details (services, customer, employee, salon) if repository supports it.
                Appointment appointment = null;

                // Prefer a detailed fetch if available on repository
                if (_unitOfWork.Appointments is object)
                {
                    // Common repository method name convention: GetByIdWithDetailsAsync
                    // If you don't have this method, replace with GetByIdAsync.
                    try
                    {
                        appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(id, cancellationToken);
                    }
                    catch
                    {
                        // fallback to basic GetByIdAsync if specialized method not present
                        appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
                    }
                }
                else
                {
                    return Result.Failure<AppointmentDto>("Appointment repository not available");
                }

                if (appointment == null)
                    return Result.Failure<AppointmentDto>("Appointment not found");

                var dto = _mapper.Map<AppointmentDto>(appointment);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure<AppointmentDto>($"Error retrieving appointment: {ex.Message}");
            }
        }

    }
}