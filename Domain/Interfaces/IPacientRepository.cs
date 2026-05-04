using SystemSaude.Entities;

namespace SystemSaude.Domain.Interfaces;

public interface IPacientRepository : IRepository<Pacient>
{
    Task<Pacient?> GetWithAddressAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pacient?> GetByCpfOrEmailAsync(string cpf, string email, CancellationToken cancellationToken = default);
}
