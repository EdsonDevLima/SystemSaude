namespace SystemSaude.Entities
{
    public class Schedules
    {
        public Guid Id{get;set;}
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string Status {get;set;}
    }

}