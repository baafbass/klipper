// SalonManagement.Infrastructure/Data/Configurations/ScheduleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonManagement.API.Domain.Entities;

namespace SalonManagement.API.Configurations
{
    public class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
    {
        public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
        {
            builder.ToTable("EmployeeSchedules");

            builder.HasKey(es => es.Id);

            builder.HasIndex(es => new { es.EmployeeId, es.DayOfWeek });
        }
    }

    public class SalonWorkingHoursConfiguration : IEntityTypeConfiguration<SalonWorkingHours>
    {
        public void Configure(EntityTypeBuilder<SalonWorkingHours> builder)
        {
            builder.ToTable("SalonWorkingHours");

            builder.HasKey(wh => wh.Id);

            builder.HasIndex(wh => new { wh.SalonId, wh.DayOfWeek }).IsUnique();
        }
    }
}