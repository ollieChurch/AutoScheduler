using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoScheduler.Services;
using AutoScheduler.Models;

namespace AutoScheduler.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public JsonFileBacklogItemService BacklogService;
    public IEnumerable<BacklogItem> Backlog { get; private set;}

    public IndexModel(
        ILogger<IndexModel> logger,
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
