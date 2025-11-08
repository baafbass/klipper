// SalonManagement.Infrastructure/Data/Configurations/SalonConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonManagement.API.Domain.Entities;

namespace SalonManagement.API.Configurations
{
    public class SalonConfiguration : IEntityTypeConfiguration<Salon>
    {
        public void Configure(EntityTypeBuilder<Salon> builder)
        {
            builder.ToTable("Salons");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.Property(s => s.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.City);
            builder.HasIndex(s => s.Email).IsUnique();

            builder.HasMany(s => s.Employees)
                .WithOne(e => e.Salon)
                .HasForeignKey(e => e.SalonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Services)
                .WithOne(sv => sv.Salon)
                .HasForeignKey(sv => sv.SalonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.WorkingHours)
                .WithOne(wh => wh.Salon)
                .HasForeignKey(wh => wh.SalonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}