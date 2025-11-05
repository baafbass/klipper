// SalonManagement.Infrastructure/Data/Configurations/UserConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonManagement.Core.Entities;

namespace SalonManagement.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Notes)
                .HasMaxLength(2000);

            builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.PhoneNumber);

            builder.HasMany(c => c.Appointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Specializations)
                .HasMaxLength(1000);

            builder.Property(e => e.CommissionRate)
                .HasPrecision(5, 2);

            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => new { e.SalonId, e.IsActive });

            builder.HasMany(e => e.EmployeeServices)
                .WithOne(es => es.Employee)
                .HasForeignKey(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Schedules)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Appointments)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class SalonManagerConfiguration : IEntityTypeConfiguration<SalonManager>
    {
        public void Configure(EntityTypeBuilder<SalonManager> builder)
        {
            builder.ToTable("SalonManagers");

            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(sm => sm.Email).IsUnique();
        }
    }
}