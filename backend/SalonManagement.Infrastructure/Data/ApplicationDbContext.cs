
// SalonManagement.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SalonManagement.Core.Entities;

namespace SalonManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SalonManager> SalonManagers { get; set; }
        public DbSet<Salon> Salons { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }
        public DbSet<EmployeeService> EmployeeServices { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<SalonWorkingHours> SalonWorkingHours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filter for soft delete
            modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SalonManager>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Salon>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);

            // ---- Prevent multiple cascade paths for AppointmentServices ----
            modelBuilder.Entity<AppointmentService>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Appointment)
                      .WithMany(a => a.AppointmentServices)
                      .HasForeignKey(e => e.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade); // Appointment -> AppointmentServices cascade (ok)

                entity.HasOne(e => e.Service)
                      .WithMany(s => s.AppointmentServices)
                      .HasForeignKey(e => e.ServiceId)
                      // Burada CASCADE yerine RESTRICT/NO ACTION kullanıyoruz:
                      .OnDelete(DeleteBehavior.Restrict); // veya DeleteBehavior.NoAction
            });
        }
    }
}