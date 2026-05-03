using Microsoft.EntityFrameworkCore;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class PacientRepository : Repository<Pacient>, IPacientRepository
{
    public PacientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Pacient?> GetWithAddressAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Pacient
            .Include(pacient => pacient.Address)
            .Include(pacient => pacient.Consults)
            .FirstOrDefaultAsync(pacient => pacient.Id == id, cancellationToken);
    }
}
