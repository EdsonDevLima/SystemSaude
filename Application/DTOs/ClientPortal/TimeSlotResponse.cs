namespace SystemSaude.Application.DTOs.ClientPortal;

public class TimeSlotResponse
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool Available { get; set; }
}
