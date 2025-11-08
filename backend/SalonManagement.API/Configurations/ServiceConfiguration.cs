// SalonManagement.Infrastructure/Data/Configurations/ServiceConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonManagement.API.Domain.Entities;

namespace SalonManagement.API.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.Property(s => s.Price)
                .HasPrecision(10, 2);

            builder.Property(s => s.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => new { s.SalonId, s.IsActive });
            builder.HasIndex(s => s.Category);

            builder.HasMany(s => s.EmployeeServices)
                .WithOne(es => es.Service)
                .HasForeignKey(es => es.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class EmployeeServiceConfiguration : IEntityTypeConfiguration<EmployeeService>
    {
        public void Configure(EntityTypeBuilder<EmployeeService> builder)
        {
            builder.ToTable("EmployeeServices");

            builder.HasKey(es => es.Id);

            builder.HasIndex(es => new { es.EmployeeId, es.ServiceId }).IsUnique();
        }
    }
}