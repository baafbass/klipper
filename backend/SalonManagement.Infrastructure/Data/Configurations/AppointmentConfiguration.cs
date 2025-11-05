// SalonManagement.Infrastructure/Data/Configurations/AppointmentConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonManagement.Core.Entities;

namespace SalonManagement.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.TotalPrice)
                .HasPrecision(10, 2);

            builder.Property(a => a.Notes)
                .HasMaxLength(2000);

            builder.Property(a => a.CancellationReason)
                .HasMaxLength(500);

            builder.HasIndex(a => new { a.CustomerId, a.AppointmentDate });
            builder.HasIndex(a => new { a.EmployeeId, a.AppointmentDate });
            builder.HasIndex(a => new { a.SalonId, a.AppointmentDate });
            builder.HasIndex(a => a.Status);

            builder.HasMany(a => a.AppointmentServices)
                .WithOne(aps => aps.Appointment)
                .HasForeignKey(aps => aps.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AppointmentServiceConfiguration : IEntityTypeConfiguration<AppointmentService>
    {
        public void Configure(EntityTypeBuilder<AppointmentService> builder)
        {
            builder.ToTable("AppointmentServices");

            builder.HasKey(aps => aps.Id);

            builder.Property(aps => aps.Price)
                .HasPrecision(10, 2);
        }
    }
}