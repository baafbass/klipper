// SalonManagement.Application/Services/SalonService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalonManagement.API.DTOs;
using SalonManagement.API.Domain.Common;
using SalonManagement.API.Domain.Entities;
using SalonManagement.API.Repositories.Interfaces;
using SalonManagement.API.Data; // your ApplicationDbContext namespace

namespace SalonManagement.API.Services
{
    public class SalonService : ISalonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SalonService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<IEnumerable<SalonDto>>> GetAllSalonsAsync(CancellationToken cancellationToken = default)
        {
            var salons = await _context.Salons
                .Where(s => s.IsActive)
                .Include(s => s.WorkingHours)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtoList = _mapper.Map<IEnumerable<SalonDto>>(salons);
            return Result.Success(dtoList);
        }

        public async Task<Result<SalonDto>> GetSalonByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var salon = await _context.Salons
                .Include(s => s.WorkingHours)
                .Include(s => s.Services)
                .Include(s => s.Employees)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (salon == null)
                return Result.Failure<SalonDto>($"Salon with id '{id}' not found.");

            var dto = _mapper.Map<SalonDto>(salon);
            return Result.Success(dto);
        }

        public async Task<Result<SalonDto>> CreateSalonAsync(CreateSalonDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                return Result.Failure<SalonDto>("CreateSalonDto cannot be null.");

            // Basic validation - expand as needed or move to FluentValidation
            if (string.IsNullOrWhiteSpace(dto.Name)) return Result.Failure<SalonDto>("Salon name is required.");
            if (string.IsNullOrWhiteSpace(dto.Address)) return Result.Failure<SalonDto>("Address is required.");
            if (string.IsNullOrWhiteSpace(dto.City)) return Result.Failure<SalonDto>("City is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber)) return Result.Failure<SalonDto>("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return Result.Failure<SalonDto>("Email is required.");

            // Create entity using domain ctor to keep invariants
            var salon = new Salon(dto.Name, dto.Address, dto.City, dto.PhoneNumber, dto.Email);
            // Description is optional on ctor, set via UpdateInfo or reflection; using UpdateInfo preserves MarkAsUpdated behavior
            salon.UpdateInfo(dto.Name, dto.Description ?? string.Empty, dto.Address, dto.City, dto.PhoneNumber, dto.Email);

            await _context.Salons.AddAsync(salon, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // reload including working hours for mapping (likely empty but consistent)
            var created = await _context.Salons
                .Include(s => s.WorkingHours)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == salon.Id, cancellationToken);

            var resultDto = _mapper.Map<SalonDto>(created);
            return Result.Success(resultDto);
        }

        public async Task<Result<SalonDto>> UpdateSalonAsync(Guid id, UpdateSalonDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                return Result.Failure<SalonDto>("UpdateSalonDto cannot be null.");

            var salon = await _context.Salons
                .Include(s => s.WorkingHours)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (salon == null)
                return Result.Failure<SalonDto>($"Salon with id '{id}' not found.");

            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.Name)) return Result.Failure<SalonDto>("Salon name is required.");
            if (string.IsNullOrWhiteSpace(dto.Address)) return Result.Failure<SalonDto>("Address is required.");
            if (string.IsNullOrWhiteSpace(dto.City)) return Result.Failure<SalonDto>("City is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber)) return Result.Failure<SalonDto>("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return Result.Failure<SalonDto>("Email is required.");

            salon.UpdateInfo(
                dto.Name,
                dto.Description ?? string.Empty,
                dto.Address,
                dto.City,
                dto.PhoneNumber,
                dto.Email
            );

            _context.Salons.Update(salon);
            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _context.Salons
                .Include(s => s.WorkingHours)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            var dtoResult = _mapper.Map<SalonDto>(updated);
            return Result.Success(dtoResult);
        }

        public async Task<Result> DeleteSalonAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var salon = await _context.Salons.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (salon == null)
                return Result.Failure($"Salon with id '{id}' not found.");

            // Soft-delete by design (your Salon entity exposes Deactivate)
            salon.Deactivate();
            _context.Salons.Update(salon);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<SalonManagerDto>> AddManagerAsync(Guid salonId, SalonManagerRequestDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                return Result.Failure<SalonManagerDto>("Request body cannot be null.");

            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.Email)) return Result.Failure<SalonManagerDto>("Email is required.");
            if (string.IsNullOrWhiteSpace(dto.FirstName)) return Result.Failure<SalonManagerDto>("First name is required.");
            if (string.IsNullOrWhiteSpace(dto.LastName)) return Result.Failure<SalonManagerDto>("Last name is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber)) return Result.Failure<SalonManagerDto>("Phone number is required.");

            // Ensure salon exists
            var salon = await _context.Salons.FirstOrDefaultAsync(s => s.Id == salonId, cancellationToken);
            if (salon == null) return Result.Failure<SalonManagerDto>($"Salon with id '{salonId}' not found.");

            // Check email uniqueness across concrete user tables that are part of the model
            var email = dto.Email.Trim().ToLowerInvariant();

            var existsInSystemAdmins = await _context.SystemAdmins.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
            if (existsInSystemAdmins) return Result.Failure<SalonManagerDto>("Email already in use.");

            var existsInSalonManagers = await _context.SalonManagers.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
            if (existsInSalonManagers) return Result.Failure<SalonManagerDto>("Email already in use.");

            var existsInCustomers = await _context.Customers.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
            if (existsInCustomers) return Result.Failure<SalonManagerDto>("Email already in use.");

            var existsInEmployees = await _context.Employees.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
            if (existsInEmployees) return Result.Failure<SalonManagerDto>("Email already in use.");

            // Create SalonManager using domain ctor
            var manager = new SalonManager(dto.Email, dto.FirstName, dto.LastName, dto.PhoneNumber, salonId);

            // Set password (hash)
            if (string.IsNullOrWhiteSpace(dto.Password))
                return Result.Failure<SalonManagerDto>("Password is required.");

            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            manager.SetPassword(hashed);

            // Persist
            await _context.SalonManagers.AddAsync(manager, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Return DTO
            var resultDto = new SalonManagerDto
            {
                Id = manager.Id,
                Email = manager.Email,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                PhoneNumber = manager.PhoneNumber,
                SalonId = manager.SalonId,
                IsActive = manager.IsActive
            };

            return Result.Success(resultDto);
        }
    }
}
