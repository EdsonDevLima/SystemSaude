using SystemSaude.Entities;

namespace SystemSaude.Entities{
public class Consult
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Pacient Pacient { get; set; } = null!;

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public string Status { get; set; } = "scheduled";

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}}