using Microsoft.EntityFrameworkCore;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class SchedulesRepository : Repository<Schedules>, ISchedulesRepository
{
    public SchedulesRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Schedules>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await Context.Schedules
            .Where(schedule => schedule.DoctorId == doctorId)
            .ToListAsync(cancellationToken);
    }
}
