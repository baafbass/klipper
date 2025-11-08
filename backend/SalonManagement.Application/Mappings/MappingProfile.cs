//// SalonManagement.Application/Mappings/MappingProfile.cs
//using AutoMapper;
//using SalonManagement.Application.DTOs;
//using SalonManagement.Application.DTOs.Auth;
//using SalonManagement.Core.Entities;
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