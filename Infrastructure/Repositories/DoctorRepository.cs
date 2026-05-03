using Microsoft.EntityFrameworkCore;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Doctor?> GetWithSchedulesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Doctor
            .Include(doctor => doctor.Schedules)
            .Include(doctor => doctor.Consults)
            .FirstOrDefaultAsync(doctor => doctor.Id == id, cancellationToken);
    }
}
