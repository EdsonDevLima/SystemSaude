using SystemSaude.Entities;

namespace SystemSaude.Domain.Interfaces;

public interface IConsultRepository : IRepository<Consult>
{
    Task<IReadOnlyList<Consult>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Consult>> GetByPacientIdAsync(Guid pacientId, CancellationToken cancellationToken = default);
}
