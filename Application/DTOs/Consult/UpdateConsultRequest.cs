namespace SystemSaude.Application.DTOs.Consult;

public class UpdateConsultRequest
{
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = "scheduled";
    public string? Notes { get; set; }
}
