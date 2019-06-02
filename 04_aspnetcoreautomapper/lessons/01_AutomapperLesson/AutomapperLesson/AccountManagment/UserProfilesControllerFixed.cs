using AutoMapper;
using AutomapperLesson.AccountManagment.ViewModels;
using AutomapperLesson.Domain.AccountManagement;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AutomapperLesson.AccountManagment
{
    [ApiController]
    [Route("api/v2/userprofiles")]
    public class UserProfilesControllerFixed : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        // Eine konfigurierte Automapper-Instanz lässt sich vom DI-System anfordern
        public UserProfilesControllerFixed(IUserProfileRepository userProfileRepository, IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserProfileViewModel>> GetAllProfiles()
        {
            return Ok(_mapper.Map<IEnumerable<UserProfileViewModel>>(_userProfileRepository.All()));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(string id, [FromBody] UserProfileEditModel userProfileUpdate)
        {
            var userProfile = _userProfileRepository.GetById(id);
            if (userProfile == null)
            {
                return BadRequest();
            }

            _mapper.Map(userProfileUpdate, userProfile);

            _userProfileRepository.Update(userProfile);
            return Ok(userProfile);
        }
    }
}
