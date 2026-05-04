namespace SystemSaude.Application.DTOs.ClientPortal;

public class DoctorAvailabilityResponse
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Speciality { get; set; } = string.Empty;
    public IReadOnlyList<DayAvailabilityResponse> Days { get; set; } = [];
}
