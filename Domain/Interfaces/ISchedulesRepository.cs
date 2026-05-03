using SystemSaude.Entities;

namespace SystemSaude.Domain.Interfaces;

public interface ISchedulesRepository : IRepository<Schedules>
{
    Task<IReadOnlyList<Schedules>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
}
