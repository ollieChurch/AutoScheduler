using AutoScheduler.Models;
using AutoScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoScheduler.Pages;
public class BacklogModel : PageModel
{
    private readonly ILogger<BacklogModel> _logger;

    public BacklogModel(ILogger<BacklogModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
