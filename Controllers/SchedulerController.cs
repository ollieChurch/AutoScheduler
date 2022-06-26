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
        [Route("createSchedule")]
        public List<ScheduleEntry> CreateNewSchedule([FromQuery] string startTime, [FromQuery] string endTime)
        {
            return SchedulerService.GetNewSchedule(startTime, endTime);
        }

    }
}
