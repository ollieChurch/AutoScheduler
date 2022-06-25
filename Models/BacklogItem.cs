using System.Text.Json;

namespace AutoScheduler.Models
{
    public class BacklogItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Length { get; set; }
        public string? Priority { get; set; }
        public bool? Completed { get; set; }

        public override string ToString() => JsonSerializer.Serialize<BacklogItem>(this);

        // public BacklogItem(string newId, string name, string length, string priority)
        // {
        //     this.Id = newId;
        //     this.Name = name;
        //     this.Length = length;
        //     this.Priority = priority;
        // }
    }
}
