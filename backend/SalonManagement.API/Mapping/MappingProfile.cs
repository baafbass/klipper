// SalonManagement.API/Mapping/MappingProfile.cs
using AutoMapper;
using SalonManagement.API.DTOs;
using SalonManagement.API.DTOs.Auth;
using SalonManagement.API.Domain.Entities;
using System.Globalization;

namespace SalonManagement.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<Customer, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Employee, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<SalonManager, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<SystemAdmin, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon != null ? src.Salon.Name : string.Empty))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src =>
                    src.EmployeeServices
                        .Where(es => es.IsActive)
                        .Select(es => es.Service)));

            CreateMap<SystemAdmin, SystemAdminDto>()
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // Salon Mappings
            CreateMap<Salon, SalonDto>()
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours));

            // CreateSalonDto -> Salon: map properties explicitly instead of calling ctor in expression tree
            CreateMap<CreateSalonDto, Salon>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
                // if your Salon entity has a constructor that sets defaults, AutoMapper will still be able to create the destination


            CreateMap<SalonWorkingHours, WorkingHoursDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src =>
                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)src.DayOfWeek)));

            // Service Mappings
            CreateMap<Service, ServiceDto>();

            // add near the Service -> ServiceDto mapping
            CreateMap<Service, ServiceWithEmployeesDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Employees, o => o.MapFrom(s =>
                    s.EmployeeServices
                     .Where(es => es.IsActive && es.Employee != null && es.Employee.IsActive)
                     .Select(es => es.Employee)  // maps to UserDto via existing mapping
                ));


            CreateMap<CreateServiceDto, Service>()
                .ConvertUsing(src => new Service(
                    src.SalonId,
                    src.Name,
                    src.Description,
                    src.DurationMinutes,
                    src.Price,
                    src.Category));

            // Appointment Mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.GetFullName()))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.GetFullName()))
                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.AppointmentServices));

            CreateMap<AppointmentService, AppointmentServiceDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));

            // Schedule Mappings
            CreateMap<EmployeeSchedule, EmployeeScheduleDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src =>
                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)src.DayOfWeek)));

            CreateMap<CreateEmployeeScheduleDto, EmployeeSchedule>()
                .ConvertUsing(src => new EmployeeSchedule(
                    src.EmployeeId,
                    src.DayOfWeek,
                    src.StartTime,
                    src.EndTime));

            CreateMap<AppointmentService, AppointmentServiceDto>()
                .ForMember(d => d.ServiceId, o => o.MapFrom(s => s.ServiceId))
                .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.Service != null ? s.Service.Name : string.Empty))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes));

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(d => d.CustomerName, o => o.MapFrom(a => a.Customer != null ? a.Customer.GetFullName() : string.Empty))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(a => a.Employee != null ? a.Employee.GetFullName() : string.Empty))
                .ForMember(d => d.SalonName, o => o.MapFrom(a => a.Salon != null ? a.Salon.Name : string.Empty))
                .ForMember(d => d.Services, o => o.MapFrom(a => a.AppointmentServices))
                .ForMember(d => d.StartTime, o => o.MapFrom(a => a.StartTime))
                .ForMember(d => d.EndTime, o => o.MapFrom(a => a.EndTime))
                .ForMember(d => d.Status, o => o.MapFrom(a => a.Status.ToString()));

            CreateMap<AvailableTimeSlotDto, AvailableTimeSlotDto>(); // identity mapping - not necessary but safe

            // Map Service -> ServiceWithEmployeesDto (makes Employees list from EmployeeServices)
            CreateMap<Service, ServiceWithEmployeesDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                // map employees who can perform the service
                .ForMember(d => d.Employees, o => o.MapFrom(s =>
                    s.EmployeeServices
                        .Where(es => es.IsActive && es.Employee != null && es.Employee.IsActive)
                        .Select(es => es.Employee)
                ));

            // Map Salon -> SalonDetailsDto (include services and workinghours)
            CreateMap<Salon, SalonDetailsDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Services, o => o.MapFrom(s => s.Services.Where(sv => sv.IsActive)))
                .ForMember(d => d.WorkingHours, o => o.MapFrom(s => s.WorkingHours));
        }
    }
}