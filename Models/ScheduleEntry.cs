using System.Text.Json;

namespace AutoScheduler.Models
{
    public class ScheduleEntry
    {
        public string? StartTime { get; set; }
        public string? FinishTime { get; set; }
        public string? TaskId { get; set; }
        public string? Name { get; set; }

        public override string ToString() => JsonSerializer.Serialize<ScheduleEntry>(this);
    }
}