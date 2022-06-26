using System.Text.Json;

namespace AutoScheduler.Models
{
    public class ScheduleEntry
    {
        public string? StartTaskTime { get; set; }
        public string? FinishTaskTime { get; set; }
        public string? TaskId { get; set; }
        public string? TaskName { get; set; }

        public ScheduleEntry(string start, string finish, string id, string name)
        {
            this.StartTaskTime = start;
            this.FinishTaskTime =finish;
            this.TaskId = id;
            this.TaskName = name;
        }

        public override string ToString() => JsonSerializer.Serialize<ScheduleEntry>(this);
    }
}