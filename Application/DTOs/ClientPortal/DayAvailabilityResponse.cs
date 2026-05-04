namespace SystemSaude.Application.DTOs.ClientPortal;

public class DayAvailabilityResponse
{
    public int DayOfWeek { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public IReadOnlyList<TimeSlotResponse> Slots { get; set; } = [];
}
