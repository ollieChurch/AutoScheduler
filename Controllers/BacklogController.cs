using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoScheduler.Services;
using AutoScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacklogController : ControllerBase
    {
        public BacklogController(JsonFileBacklogItemService backlogItemService) {
            this.BacklogItemService = backlogItemService;
        }

        public JsonFileBacklogItemService BacklogItemService { get; }

        [HttpGet]
        public IEnumerable<BacklogItem> Get()
        {
           return BacklogItemService.GetBacklog();
        }

        [HttpPut]
        [Route("changeStatus")]
        public ActionResult ChangeStatus([FromQuery]  string backlogId)
        {
            BacklogItemService.ChangeCompletedStatus(backlogId);
            return Ok();
        }

        [HttpPost]
        [Route("addItem")]
        public ActionResult AddItem([FromForm]  string task, [FromForm] string length, [FromForm] string priority, [FromForm] string category)
        {
            BacklogItemService.AddItemToBacklog(task, length, priority, category);
            return Redirect("/Backlog");
        }

        [HttpPut]
        [Route("removeItem")]
        public ActionResult RemoveItem([FromQuery] string backlogId)
        {
            BacklogItemService.RemoveItemFromBacklog(backlogId);
            return Redirect("/Backlog");
        }
    }
}
