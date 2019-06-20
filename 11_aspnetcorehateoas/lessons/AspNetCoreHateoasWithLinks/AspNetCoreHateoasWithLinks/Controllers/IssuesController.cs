using AspNetCoreHateoasWithLinks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreHateoasWithLinks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private static readonly Dictionary<string, Issue> _tasks = new Dictionary<string, Issue>
        {
            ["1"] = Issue.CreateNew("1")
        };

        [HttpGet("{id}", Name = "GetTaskById")]
        public ActionResult<IEnumerable<IssueDto>> GetTaskById(string id)
        {
            Issue task = _tasks.TryGetValue(id, out Issue t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPut("inprogress/{id}", Name = "SetTaskInProgress")]
        public ActionResult<IEnumerable<IssueDto>> SetTaskInProgress(string id)
        {
            Issue task = _tasks.TryGetValue(id, out Issue t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            if (task.State >= IssueStates.InProgress)
            {
                return BadRequest();
            }

            task.State = IssueStates.InProgress;
            return Ok(task);
        }

        [HttpPut("done/{id}", Name = "SetTaskDone")]
        public ActionResult<IEnumerable<IssueDto>> SetTaskDone(string id)
        {
            Issue task = _tasks.TryGetValue(id, out Issue t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            if (task.State >= IssueStates.Done)
            {
                return BadRequest();
            }

            task.State = IssueStates.Done;
            return Ok(task);
        }
    }
}
