using AutoScheduler.Models;
using AutoScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoScheduler.Pages;

public class BacklogModel : PageModel
{
    private readonly ILogger<BacklogModel> _logger;
    public JsonFileBacklogItemService BacklogService;
    public IEnumerable<BacklogItem> Backlog { get; private set;}

    public BacklogModel(
        ILogger<BacklogModel> logger,
        JsonFileBacklogItemService backlogService)
    {
        _logger = logger;
        BacklogService = backlogService;
    }

    public void OnGet()
    {
        Backlog = BacklogService.GetBacklog();
    }
}
