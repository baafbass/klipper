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
        }
    }
}







//// SalonManagement.Application/Mappings/MappingProfile.cs
//using AutoMapper;
//using SalonManagement.API.DTOs;
//using SalonManagement.API.DTOs.Auth;
//using SalonManagement.API.Domain.Entities;
//using System.Globalization;

//namespace SalonManagement.Application.Mappings
//{
//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            // User Mappings
//            CreateMap<Customer, UserDto>()
//                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

//            CreateMap<Employee, UserDto>()
//                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

//            CreateMap<SalonManager, UserDto>()
//                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

//            CreateMap<Customer, CustomerDto>()
//                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

//            CreateMap<Employee, EmployeeDto>()
//                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
//                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon != null ? src.Salon.Name : string.Empty))
//                .ForMember(dest => dest.Specializations, opt => opt.MapFrom(src =>
//                    string.IsNullOrEmpty(src.Specializations)
//                        ? new List<string>()
//                        : System.Text.Json.JsonSerializer.Deserialize<List<string>>(src.Specializations)))
//                .ForMember(dest => dest.Services, opt => opt.MapFrom(src =>
//                    src.EmployeeServices
//                        .Where(es => es.IsActive)
//                        .Select(es => es.Service)));

//            // Salon Mappings
//            CreateMap<Salon, SalonDto>()
//                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours));

//            CreateMap<CreateSalonDto, Salon>()
//                .ConstructUsing(src => new Salon(
//                    src.Name,
//                    src.Address,
//                    src.City,
//                    src.PhoneNumber,
//                    src.Email))
//                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

//            CreateMap<SalonWorkingHours, WorkingHoursDto>()
//                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src =>
//                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)src.DayOfWeek)));

//            // Service Mappings
//            CreateMap<Service, ServiceDto>();

//            CreateMap<CreateServiceDto, Service>()
//                .ConstructUsing(src => new Service(
//                    src.SalonId,
//                    src.Name,
//                    src.Description,
//                    src.DurationMinutes,
//                    src.Price,
//                    src.Category));

//            // Appointment Mappings
//            CreateMap<Appointment, AppointmentDto>()
//                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.GetFullName()))
//                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.GetFullName()))
//                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon.Name))
//                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
//                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.AppointmentServices));

//            CreateMap<AppointmentService, AppointmentServiceDto>()
//                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));

//            // Schedule Mappings
//            CreateMap<EmployeeSchedule, EmployeeScheduleDto>()
//                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src =>
//                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)src.DayOfWeek)));

//            CreateMap<CreateEmployeeScheduleDto, EmployeeSchedule>()
//                .ConstructUsing(src => new EmployeeSchedule(
//                    src.EmployeeId,
//                    src.DayOfWeek,
//                    src.StartTime,
//                    src.EndTime));
//        }
//    }
//}