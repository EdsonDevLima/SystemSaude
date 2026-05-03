using Microsoft.EntityFrameworkCore;
using SystemSaude.Entities;

namespace SystemSaude.Infrastructure.Data{
    public class AppDbContext:DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) {}
        public DbSet<Address> Address{get;set;}
        public DbSet<Consult> Consult{get;set;}
        
        public DbSet<Doctor> Doctor{get;set;}

        public DbSet<Pacient> Pacient{get;set;}

        public DbSet<Schedules>Schedules{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>()
                .HasMany(doctor => doctor.Consults)
                .WithOne(consult => consult.Doctor)
                .HasForeignKey(consult => consult.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasMany(doctor => doctor.Schedules)
                .WithOne(schedule => schedule.Doctor)
                .HasForeignKey(schedule => schedule.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pacient>()
                .HasMany(pacient => pacient.Consults)
                .WithOne(consult => consult.Pacient)
                .HasForeignKey(consult => consult.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pacient>()
                .HasOne(pacient => pacient.Address)
                .WithOne(address => address.Pacient)
                .HasForeignKey<Address>("PacientId")
                .OnDelete(DeleteBehavior.Cascade);
        }
}
}
