using SystemSaude.Application.DTOs.Consult;

namespace SystemSaude.Application.UseCases.Consults;

public interface IConsultUseCase
{
    Task<IReadOnlyList<ConsultResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ConsultResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConsultResponse>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConsultResponse>> GetByPacientIdAsync(Guid pacientId, CancellationToken cancellationToken = default);
    Task<ConsultResponse> CreateAsync(CreateConsultRequest request, CancellationToken cancellationToken = default);
    Task<ConsultResponse?> UpdateAsync(Guid id, UpdateConsultRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
