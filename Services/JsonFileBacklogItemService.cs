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
        public JsonFileBacklogItemService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

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

        public void AddItemToBacklog(string name, string length, string priority)
        {
            var backlog = GetBacklog().ToList();
            var prevId = backlog.Count == 0 || backlog == null ? "0" : backlog.Last().Id;

            try
            {
                var newId = (int.Parse(prevId) + 1).ToString();
                var newItem = new BacklogItem();
                newItem.Id = newId;
                newItem.Name = name;
                newItem.Length = length;
                newItem.Priority = priority;
                backlog.Add(newItem);

                Console.WriteLine(newItem);

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
                Console.WriteLine("failed to remove from list");
            }
        }

        
    }
}