using System.Text.Json;
using AutoScheduler.Models;

namespace AutoScheduler.Services
{
    public class JsonFileSchedulerService
    {
        public JsonFileBacklogItemService BacklogService;
        public IWebHostEnvironment WebHostEnvironment { get; }

        public JsonFileSchedulerService(JsonFileBacklogItemService backlogService, IWebHostEnvironment webHostEnvironment)
        {
            BacklogService = backlogService;
            WebHostEnvironment = webHostEnvironment;
        }

        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "schedule.json"); }
        }

        public IEnumerable<ScheduleEntry> GetSchedule()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                var result = JsonSerializer.Deserialize<ScheduleEntry[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return result != null ? result : default!;
            }
        }

        public List<ScheduleEntry> CreateNewSchedule(string start, string finish)
        {
            var newSchedule = new List<ScheduleEntry>();
            var backlog = BacklogService.GetBacklog().ToList().FindAll(x => x.Completed == false || x.Completed == null);
            var minsRemaining = (TimeOnly.Parse(finish) - TimeOnly.Parse(start)).TotalMinutes;
            var scheduleTime = TimeOnly.Parse(start);
            var priorityLevels = new List<string> {"high", "mid", "low"};
            var breakTimes = CalculateBreaks(minsRemaining, scheduleTime);
            
            foreach(var level in priorityLevels)
            {
                foreach(var task in backlog.FindAll(x => x.Priority == level))
                {
                    var breakLengthInMins = 20;
                    var includeBreaks = true;
                    if (
                        includeBreaks && 
                        breakTimes.Count() > 0 &&
                        scheduleTime >= breakTimes[0] && 
                        breakLengthInMins <= minsRemaining
                    ){
                        var newScheduleTime = scheduleTime.AddMinutes(breakLengthInMins);
                        
                        newSchedule.Add(CreateNewScheduleEntry(
                            scheduleTime,
                            newScheduleTime,
                            "break",
                            null
                        ));

                        minsRemaining = minsRemaining - breakLengthInMins;
                        scheduleTime = newScheduleTime;
                        breakTimes.RemoveAt(0);
                    }

                    var lengthInMins = GetTaskLengthInMins(task.Length);
                    if (lengthInMins <= minsRemaining)
                    {
                        var newScheduleTime = scheduleTime.AddMinutes(lengthInMins);

                        newSchedule.Add(CreateNewScheduleEntry(
                            scheduleTime,
                            newScheduleTime,
                            task.Id,
                            task.Name
                        ));

                        minsRemaining = minsRemaining - lengthInMins;
                        scheduleTime = newScheduleTime;
                    }                    
                }
            }

            using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
            {
                JsonSerializer.Serialize<List<ScheduleEntry>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    newSchedule
                );
            }
            
            return newSchedule;
        }

        public bool CheckIfScheduleComplete()
        {
            foreach(var task in GetSchedule())
            {
                if (task.TaskId == null || task.TaskId == "break")
                {
                    return false;
                }

                var backlogMatchComplete = Convert.ToBoolean(BacklogService.GetMatchingBacklogItem(task.TaskId).Completed);
                if (!backlogMatchComplete)
                {
                    return false;
                }
            }

            return true;
        }

        public void RemoveItemFromSchedule(string id)
        {
            var schedule = GetSchedule();
            if (schedule.Any(x => x.TaskId == id))
            {
                var matchingItem = schedule.First(x => x.TaskId == id);
                matchingItem.TaskId = null;

                using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
                {
                    JsonSerializer.Serialize<IEnumerable<ScheduleEntry>>(
                        new Utf8JsonWriter(outputStream, new JsonWriterOptions
                        {
                            SkipValidation = true,
                            Indented = true
                        }),
                        schedule
                    );
                }
            }
        }
        
        public void DeleteSchedule() {
            var schedule = GetSchedule().ToList();
            schedule.RemoveAll(x => x != null);

            using (var outputStream = File.Open(JsonFileName, FileMode.Truncate))
            {
                JsonSerializer.Serialize<List<ScheduleEntry>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    schedule
                );
            }
        }

        public ScheduleEntry CreateNewScheduleEntry(TimeOnly scheduleTime, TimeOnly newScheduleTime, string? taskId, string? name)
        {
            var newEntry = new ScheduleEntry();
            newEntry.StartTime = scheduleTime.ToString();
            newEntry.FinishTime = newScheduleTime.ToString();
            newEntry.TaskId = taskId;
            newEntry.Name = name;
            return newEntry;
        }

        public List<TimeOnly> CalculateBreaks(double sessionMins, TimeOnly startTime)
        {
            var minimumSessionLength = 180;
            var numOfBreaks = Convert.ToInt16(Math.Floor(sessionMins / minimumSessionLength));
            var breakInterval = Math.Floor(sessionMins / (numOfBreaks + 1));
            var breakTimes = new List<TimeOnly>();

            for(var i = 1; i <= numOfBreaks; i++)
            {
                if (numOfBreaks > 0)
                {
                    var newBreak = startTime.AddMinutes(breakInterval * i);
                    breakTimes.Add(newBreak);
                    Console.WriteLine($"new break: {newBreak}");
                } 
            }

            return breakTimes;
        }

        public int GetTaskLengthInMins(string lengthString)
        {
            int result;
            switch(lengthString)
            {
                case "very long":
                    result = 120;
                break;

                case "long":
                    result = 60;
                break;

                case "medium":
                    result = 30;
                break;

                case "short":
                    result = 15;
                break;

                default:
                    result = 15;
                break;
            }

            return result;
        }
    }
}