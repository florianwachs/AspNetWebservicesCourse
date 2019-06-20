using AspNetCoreHateoasWithLinks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
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
        private readonly IUrlHelper _urlHelper;

        public IssuesController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<IssueDto>> GetIssueById(string id)
        {
            Issue task = _tasks.TryGetValue(id, out Issue t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            return Ok(GetDtoWithLinks(task));
        }

        [HttpPut("inprogress/{id}")]
        public ActionResult<IEnumerable<IssueDto>> SetIssueInProgress(string id)
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
            return Ok(GetDtoWithLinks(task));
        }

        [HttpPut("done/{id}")]
        public ActionResult<IEnumerable<IssueDto>> SetIssueDone(string id)
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
            return Ok(GetDtoWithLinks(task));
        }

        [HttpPut("removed/{id}")]
        public ActionResult<IEnumerable<IssueDto>> SetIssueRemoved(string id)
        {
            Issue task = _tasks.TryGetValue(id, out Issue t) ? t : null;
            if (task == null)
            {
                return NotFound();
            }

            if (task.State == IssueStates.Done)
            {
                return BadRequest();
            }

            task.State = IssueStates.Removed;
            return Ok(GetDtoWithLinks(task));
        }

        private IssueDto GetDtoWithLinks(Issue issue)
        {
            var dto = IssueDto.From(issue);

            return dto;
        }
    }

    public class IssueLinkGenerator
    {
        private readonly IUrlHelper _urlHelper;

        public IssueLinkGenerator(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public IEnumerable<LinkDto> GetLinks(Issue issue)
        {
            yield return new LinkDto(
                       "_self",
                       _urlHelper.Action(nameof(IssuesController.GetIssueById),
                       nameof(IssuesController), new { id = issue.Id }), "GET");

            switch (issue.State)
            {
                case IssueStates.New:
                    yield return new LinkDto(
                        "Set_In_Progress",
                        _urlHelper.Action(nameof(IssuesController.SetIssueInProgress),
                        nameof(IssuesController), new { id = issue.Id }), "PUT");

                    yield return new LinkDto(
                        "Set_Removed",
                        _urlHelper.Action(nameof(IssuesController.SetIssueRemoved),
                        nameof(IssuesController), new { id = issue.Id }), "PUT");
                    break;

                case IssueStates.InProgress:
                    yield return new LinkDto(
                       "Set_Done",
                       _urlHelper.Action(nameof(IssuesController.SetIssueDone),
                       nameof(IssuesController), new { id = issue.Id }), "PUT");

                    yield return new LinkDto(
                     "Set_Removed",
                     _urlHelper.Action(nameof(IssuesController.SetIssueRemoved),
                     nameof(IssuesController), new { id = issue.Id }), "PUT");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(issue.UserId))
            {
                yield return new LinkDto(
                      "Get_User",
                      _urlHelper.Action(nameof(UsersController.GetUserById),
                      nameof(UsersController), new { id = issue.UserId }), "GET");
            }
        }
    }
}
