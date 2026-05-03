using SystemSaude.Entities;

namespace SystemSaude.Domain.Interfaces;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<Doctor?> GetWithSchedulesAsync(Guid id, CancellationToken cancellationToken = default);
}
