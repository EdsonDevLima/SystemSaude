using SystemSaude.Entities;

namespace SystemSaude.Domain.Interfaces;

public interface IPacientRepository : IRepository<Pacient>
{
    Task<Pacient?> GetWithAddressAsync(Guid id, CancellationToken cancellationToken = default);
}
