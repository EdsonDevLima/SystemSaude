namespace SystemSaude.Application.DTOs.ClientPortal;

public class DoctorOptionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Speciality { get; set; } = string.Empty;
    public bool Status { get; set; }
}
