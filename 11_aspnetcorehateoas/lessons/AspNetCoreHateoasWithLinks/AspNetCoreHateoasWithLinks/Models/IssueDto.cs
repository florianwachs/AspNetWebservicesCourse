using System;
using System.Collections.Generic;
using System.Data;

namespace AspNetCoreHateoasWithLinks.Models
{
    public class IssueDto : LinkedResourceBaseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public IssueStates State { get; set; }

        public static IssueDto From(Issue issue)
        {
            return new IssueDto
            {
                Id = issue.Id,
                Name = issue.Name,
                Description = issue.Description,
                UserId = issue.UserId,
                State = issue.State,
            };
        }        
    }
}
