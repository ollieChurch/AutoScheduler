using AutoScheduler.Services;
using AutoScheduler.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<JsonFileBacklogItemService>();
builder.Services.AddTransient<UserStatsService>();
builder.Services.AddTransient<JsonFileSchedulerService>();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapRazorPages();
app.MapBlazorHub();

// app.MapGet("/backlogItems", (context) => 
// {
//     var backlog = app.Services.GetService<JsonFileBacklogItemService>().GetBacklog();
//     var json = JsonSerializer.Serialize<IEnumerable<BacklogItem>>(backlog);
//     return context.Response.WriteAsync(json);
// });

app.Run();
