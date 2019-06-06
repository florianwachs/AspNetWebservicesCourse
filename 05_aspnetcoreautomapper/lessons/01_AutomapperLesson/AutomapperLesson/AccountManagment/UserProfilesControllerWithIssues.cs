using AutomapperLesson.Domain.AccountManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomapperLesson.AccountManagment
{
    [ApiController]
    [Route("api/v1/userprofiles")]
    public class UserProfilesControllerWithIssues : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfilesControllerWithIssues(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserProfile>> GetAllProfiles()
        {
            return Ok(_userProfileRepository.All());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(string id, [FromBody] UserProfile userProfile)
        {
            _userProfileRepository.Update(userProfile);
            return Ok(userProfile);
        }
    }
}
