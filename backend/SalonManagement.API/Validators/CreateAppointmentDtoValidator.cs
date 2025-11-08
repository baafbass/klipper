// SalonManagement.Application/Validators/CreateAppointmentDtoValidator.cs
using FluentValidation;
using SalonManagement.API.DTOs;
using SalonManagement.API.DTOs.Auth;

namespace SalonManagement.Application.Validators
{
	public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
	{
		public CreateAppointmentDtoValidator()
		{
			RuleFor(x => x.CustomerId)
				.NotEmpty().WithMessage("Customer ID is required");

			RuleFor(x => x.EmployeeId)
				.NotEmpty().WithMessage("Employee ID is required");

			RuleFor(x => x.SalonId)
				.NotEmpty().WithMessage("Salon ID is required");

			RuleFor(x => x.AppointmentDate)
				.NotEmpty().WithMessage("Appointment date is required")
				.Must(date => date >= DateTime.Today).WithMessage("Appointment date cannot be in the past");

			RuleFor(x => x.ServiceIds)
				.NotEmpty().WithMessage("At least one service must be selected")
				.Must(ids => ids != null && ids.Any()).WithMessage("At least one service must be selected");

			RuleFor(x => x.Notes)
				.MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
		}
	}

	public class CreateSalonDtoValidator : AbstractValidator<CreateSalonDto>
	{
		public CreateSalonDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Salon name is required")
				.MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

			RuleFor(x => x.Address)
				.NotEmpty().WithMessage("Address is required")
				.MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

			RuleFor(x => x.City)
				.NotEmpty().WithMessage("City is required")
				.MaximumLength(100).WithMessage("City cannot exceed 100 characters");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required")
				.Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email format");
		}
	}

	public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
	{
		public LoginRequestDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email format");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters");
		}
	}

	public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
	{
		public RegisterRequestDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email format");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters")
				.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
				.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
				.Matches(@"[0-9]").WithMessage("Password must contain at least one number");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required")
				.MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required")
				.MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required")
				.Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format");
		}
	}
}