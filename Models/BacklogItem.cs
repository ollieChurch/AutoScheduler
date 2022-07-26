using System.Text.Json;

namespace AutoScheduler.Models
{
    public class BacklogItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Length { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public bool? Completed { get; set; }
        public string? CompletedDateTime { get; set; }

        public override string ToString() => JsonSerializer.Serialize<BacklogItem>(this);
    }
}
