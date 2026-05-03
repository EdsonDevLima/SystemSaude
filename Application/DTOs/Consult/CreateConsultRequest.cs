namespace SystemSaude.Application.DTOs.Consult;

public class CreateConsultRequest
{
    public Guid DoctorId { get; set; }
    public Guid PacientId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = "scheduled";
    public string? Notes { get; set; }
}
