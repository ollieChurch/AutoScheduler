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
            using(var jsonFileReader = File.OpenText(JsonFileName))
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

            query.Completed = query.Completed == null || query.Completed == false ? true : false;

            using(var outputStream = File.Open(JsonFileName, FileMode.Truncate))
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
            var prevId = backlog.Last().Id;
            var newId = prevId is null ? "1" : (int.Parse(prevId) + 1).ToString();
            var newItem = new BacklogItem();
            newItem.Id = newId;
            newItem.Name = name;
            newItem.Length = length;
            newItem.Priority = priority;
            backlog.Add(newItem);

            Console.WriteLine(newItem);

            using(var outputStream = File.Open(JsonFileName, FileMode.Truncate))
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

        public void RemoveItemFromBacklog(string backlogId)
        {
            var backlog = GetBacklog().ToList();
            var query = backlog.First(x => x.Id == backlogId);
            var result = backlog.Remove(query);
            if (!result)
            {
                Console.WriteLine("failed to remove from list");
            }

            using(var outputStream = File.Open(JsonFileName, FileMode.Truncate))
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
    }
}