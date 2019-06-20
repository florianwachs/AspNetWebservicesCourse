using AspNetCoreHateoasWithLinks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreHateoasWithLinks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private static readonly Dictionary<string, TaskDto> _tasks = new Dictionary<string, TaskDto>
        {
            ["1"] = TaskDto.CreateNew("1")
        };

        [HttpGet("{id}", Name = "GetTaskById")]
        public ActionResult<IEnumerable<TaskDto>> GetTaskById(string id)
        {
            TaskDto task = _tasks.TryGetValue(id, out TaskDto t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPut("inprogress/{id}", Name = "SetTaskInProgress")]
        public ActionResult<IEnumerable<TaskDto>> SetTaskInProgress(string id)
        {
            TaskDto task = _tasks.TryGetValue(id, out TaskDto t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            if (task.TaskState >= TaskStates.InProgress)
            {
                return BadRequest();
            }

            task.TaskState = TaskStates.InProgress;
            return Ok(task);
        }

        [HttpPut("done/{id}", Name = "SetTaskDone")]
        public ActionResult<IEnumerable<TaskDto>> SetTaskDone(string id)
        {
            TaskDto task = _tasks.TryGetValue(id, out TaskDto t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            if (task.TaskState >= TaskStates.Done)
            {
                return BadRequest();
            }

            task.TaskState = TaskStates.Done;
            return Ok(task);
        }
    }
}
