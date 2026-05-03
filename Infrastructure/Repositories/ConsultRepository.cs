using Microsoft.EntityFrameworkCore;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class ConsultRepository : Repository<Consult>, IConsultRepository
{
    public ConsultRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Consult>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await Context.Consult
            .AsNoTracking()
            .Include(consult => consult.Doctor)
            .Include(consult => consult.Pacient)
            .Where(consult => consult.DoctorId == doctorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Consult>> GetByPacientIdAsync(Guid pacientId, CancellationToken cancellationToken = default)
    {
        return await Context.Consult
            .AsNoTracking()
            .Include(consult => consult.Doctor)
            .Include(consult => consult.Pacient)
            .Where(consult => consult.PatientId == pacientId)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Consult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Consult
            .AsNoTracking()
            .Include(consult => consult.Doctor)
            .Include(consult => consult.Pacient)
            .FirstOrDefaultAsync(consult => consult.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Consult>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Consult
            .AsNoTracking()
            .Include(consult => consult.Doctor)
            .Include(consult => consult.Pacient)
            .ToListAsync(cancellationToken);
    }
}
