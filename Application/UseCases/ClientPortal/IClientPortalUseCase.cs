using SystemSaude.Application.DTOs.ClientPortal;

namespace SystemSaude.Application.UseCases.ClientPortal;

public interface IClientPortalUseCase
{
    Task<PacientPortalResponse> RegisterPacientAsync(RegisterPacientRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorOptionResponse>> GetDoctorsAsync(CancellationToken cancellationToken = default);
    Task<DoctorAvailabilityResponse?> GetDoctorAvailabilityAsync(Guid doctorId, CancellationToken cancellationToken = default);
}
