using AutoScheduler.Models;
using AutoScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoScheduler.Pages;
public class ScheduleModel : PageModel
{
    private readonly ILogger<ScheduleModel> _logger;

    public JsonFileSchedulerService SchedulerService;
    public IEnumerable<ScheduleEntry> Schedule { get; set;}

    public ScheduleModel(
        ILogger<ScheduleModel> logger,
        JsonFileSchedulerService schedulerService)
    {
        _logger = logger;
        SchedulerService = schedulerService;
    }

    public void OnGet()
    {
        Schedule = SchedulerService.GetSchedule();
    }
}
