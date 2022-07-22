using AutoScheduler.Models;

namespace AutoScheduler.Services
{
    public class UserStatsService
    {
        public JsonFileBacklogItemService BacklogService;
        public UserStatsService(JsonFileBacklogItemService backlogService)
        {
            BacklogService = backlogService;
        }

        public int GetNumOfItemsInBacklog()
        {
            var backlog = BacklogService.GetBacklog().ToList();
            return backlog.FindAll(x => x.Completed != true).Count;
        }

        public int GetNumOfRecentlyCompleted(int prevDays)
        {
            var backlog = BacklogService.GetBacklog().ToList();
            var filteredBacklog = backlog.FindAll(x =>
                x.CompletedDateTime != null && 
                (DateTime.UtcNow.Date - DateTime.Parse(x.CompletedDateTime)).TotalDays  <= prevDays
            );
            
            return filteredBacklog.Count;
        }

        public List<UserStatistic> GetStatsOfPriority()
        {
            var backlog = BacklogService.GetBacklog().ToList();
            var result = new List<UserStatistic> {};
            var priorities = new List<string> {"high", "mid", "low"};

            foreach(string priority in priorities)
            {
                result.Add(
                    new UserStatistic(
                        $"{priority}", 
                        Convert.ToDouble(backlog.FindAll(
                            x => x.Priority == priority && x.Completed != true
                        ).Count)
                    )
                );
            }

            return result != null ? result : default!;
        }

        public List<UserStatistic> GetStatsOfLength()
        {
            var backlog = BacklogService.GetBacklog().ToList();
            var result = new List<UserStatistic> {};
            var lengths = new List<string> {"very long", "long", "medium", "short"};

             foreach(string length in lengths)
            {
                result.Add(
                    new UserStatistic(
                        $"{length}", 
                        Convert.ToDouble(backlog.FindAll(x => 
                            x.Length == length && x.Completed != true
                        ).Count)
                    )
                );            
            }

            return result != null ? result : default!;
        }
    }
}