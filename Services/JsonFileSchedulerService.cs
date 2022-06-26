using AutoScheduler.Models;

namespace AutoScheduler.Services
{
    public class JsonFileSchedulerService
    {
        public JsonFileBacklogItemService BacklogService;
        public JsonFileSchedulerService(JsonFileBacklogItemService backlogService)
        {
            BacklogService = backlogService;
        }

        public List<ScheduleEntry> GetNewSchedule(string start, string finish)
        {
            var newSchedule = new List<ScheduleEntry>();
            var backlog = BacklogService.GetBacklog().ToList().FindAll(x => x.Completed == false || x.Completed == null);
            var minsRemaining = (TimeOnly.Parse(finish) - TimeOnly.Parse(start)).TotalMinutes;
            var scheduleTime = TimeOnly.Parse(start);
            var priorityLevels = new List<string> {"high", "mid", "low"};
            
            foreach(var level in priorityLevels)
            {
                foreach(var task in backlog.FindAll(x => x.Priority == level))
                {
                    var lengthInMins = GetTaskLengthInMins(task.Length);
                    if (lengthInMins < minsRemaining)
                    {
                        var newScheduleTime = scheduleTime.AddMinutes(lengthInMins);
                        var newEntry = new ScheduleEntry
                        (
                            scheduleTime.ToString(), 
                            newScheduleTime.ToString(), 
                            task.Id, 
                            task.Name
                        );

                        newSchedule.Add(newEntry);
                        minsRemaining = minsRemaining - lengthInMins;
                        scheduleTime = newScheduleTime;
                    }                    
                }
            }
            
            Console.WriteLine(newSchedule);
            return newSchedule;
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