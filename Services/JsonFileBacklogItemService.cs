using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AutoScheduler.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.Services
{
    public class JsonFileBacklogItemService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        public JsonFileBacklogItemService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "backlog.json"); }
        }

        public IEnumerable<BacklogItem> GetBacklog()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                var result = JsonSerializer.Deserialize<BacklogItem[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return result != null ? result : default!;
            }
        }

        public BacklogItem GetMatchingBacklogItem(string id)
        {
            try
            {
                return GetBacklog().First(x => x.Id == id);
            }
            catch
            {
                Console.WriteLine("Unable to match item to backlog");
                return default!;
            }
        }

        public IEnumerable<string> GetCategories()
        {
            var categories = new List<string>();
            foreach(var item in GetBacklog())
            {
                if (item.Category != null && item.Completed != true)
                {
                    categories.Add(item.Category);
                }
            }

            return categories.Distinct();
        }

        public void ChangeCompletedStatus(string backlogId)
        {
            var backlog = GetBacklog();
            var query = backlog.First(x => x.Id == backlogId);

            if (query.Completed == false || query.Completed == null)
            {
                query.Completed = true;
                query.CompletedDateTime = DateTime.UtcNow.ToString();
            }
            else
            {
                query.Completed = false;
                query.CompletedDateTime = null;
            }

            using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
            {
                JsonSerializer.Serialize<IEnumerable<BacklogItem>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    backlog
                );
            }
        }

        public void AddItemToBacklog(string name, string length, string priority, string category)
        {
            var backlog = GetBacklog().ToList();
            var prevId = backlog.Count == 0 || backlog == null ? "0" : backlog.Last().Id;

            try
            {
                var newId = (int.Parse(prevId) + 1).ToString();
                var newItem = new BacklogItem();
                newItem.Id = newId;
                newItem.Name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                newItem.Length = length;
                newItem.Priority = priority;
                newItem.Category = char.ToUpper(category[0]) + category.Substring(1).ToLower();
                backlog.Add(newItem);

                Console.WriteLine(newItem);

                using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
                {
                    JsonSerializer.Serialize<List<BacklogItem>>(
                        new Utf8JsonWriter(outputStream, new JsonWriterOptions
                        {
                            SkipValidation = true,
                            Indented = true
                        }),
                        backlog
                    );
                }
            }
            catch
            {
                Console.WriteLine("something went wrong! No item added.");
            }
        }

        public void RemoveItemFromBacklog(string backlogId)
        {
            var backlog = GetBacklog().ToList();
            var query = backlog.First(x => x.Id == backlogId);

            try
            {
                backlog.Remove(query);

                using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
                {
                    JsonSerializer.Serialize<IEnumerable<BacklogItem>>(
                        new Utf8JsonWriter(outputStream, new JsonWriterOptions
                        {
                            SkipValidation = true,
                            Indented = true
                        }),
                        backlog
                    );
                }
            }
            catch
            {
                Console.WriteLine("failed to remove from backlog and or schedule");
            }
        }

        
    }
}