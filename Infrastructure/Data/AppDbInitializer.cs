using Microsoft.EntityFrameworkCore;
using SystemSaude.Entities;

namespace SystemSaude.Infrastructure.Data;

public static class AppDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException || ex.InnerException is not null)
        {
            throw new InvalidOperationException(
                "Nao foi possivel inicializar o banco de dados do SystemSaude. Verifique se o SQL Server esta em execucao e se a connection string aponta para a instancia correta.",
                ex);
        }

        if (!await context.Doctor.AnyAsync())
        {
            var doctors = SeedDoctors();
            await context.Doctor.AddRangeAsync(doctors);
            await context.SaveChangesAsync();
        }

        if (!await context.Schedules.AnyAsync())
        {
            var doctors = await context.Doctor.AsNoTracking().ToListAsync();
            var schedules = SeedSchedules(doctors);
            await context.Schedules.AddRangeAsync(schedules);
            await context.SaveChangesAsync();
        }
    }

    private static List<Doctor> SeedDoctors()
    {
        return
        [
            new Doctor
            {
                Id = Guid.NewGuid(),
                Name = "Dra. Ana Beatriz",
                Speciality = "Clinica Geral",
                Status = true,
                LicenseNumber = "CRM-PE-1001",
                Phone = "(81) 90000-1001"
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                Name = "Dr. Carlos Henrique",
                Speciality = "Cardiologia",
                Status = true,
                LicenseNumber = "CRM-PE-1002",
                Phone = "(81) 90000-1002"
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                Name = "Dra. Marina Costa",
                Speciality = "Dermatologia",
                Status = true,
                LicenseNumber = "CRM-PE-1003",
                Phone = "(81) 90000-1003"
            }
        ];
    }

    private static List<Schedules> SeedSchedules(IEnumerable<Doctor> doctors)
    {
        var businessDays = new[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        var schedules = new List<Schedules>();

        foreach (var doctor in doctors)
        {
            foreach (var day in businessDays)
            {
                schedules.Add(new Schedules
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctor.Id,
                    DayOfWeek = day,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(12, 0),
                    Status = "active"
                });

                schedules.Add(new Schedules
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctor.Id,
                    DayOfWeek = day,
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(17, 0),
                    Status = "active"
                });
            }
        }

        return schedules;
    }
}
