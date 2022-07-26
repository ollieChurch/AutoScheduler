using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoScheduler.Models;
using AutoScheduler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        public SchedulerController(JsonFileSchedulerService schedulerService) {
            this.SchedulerService = schedulerService;
        }

        public JsonFileSchedulerService SchedulerService { get; }

        [HttpGet]
        [Route("getSchedule")]
        public IEnumerable<ScheduleEntry> getSchedule()
        {
            return SchedulerService.GetSchedule();
        }

        [HttpPost]
        [Route("createSchedule")]
        public ActionResult CreateNewSchedule([FromForm] string startTime, [FromForm] string endTime, [FromForm] IEnumerable<string> includedCategories)
        {
            SchedulerService.CreateNewSchedule(startTime, endTime, includedCategories.ToList());
            return Redirect("/schedule");
        }

    }
}
