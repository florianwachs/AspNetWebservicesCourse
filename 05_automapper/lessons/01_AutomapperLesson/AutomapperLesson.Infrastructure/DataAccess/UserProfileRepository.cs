using AutomapperLesson.Domain.AccountManagement;
using System.Collections.Generic;

namespace AutomapperLesson.Infrastructure.DataAccess
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly Dictionary<string, UserProfile> _profiles = new Dictionary<string, UserProfile>
        {
            ["1"] = new UserProfile { Id = "1", FirstName = "Chuck", LastName = "Norris", Age = 28, IsAdmin = true, CreditCardNumber = "12345" },
            ["2"] = new UserProfile { Id = "2", FirstName = "Jason", LastName = "Bourne", Age = 74, IsAdmin = false, CreditCardNumber = "5674" },
        };

        public IReadOnlyCollection<UserProfile> All()
        {
            return _profiles.Values;
        }

        public UserProfile GetById(string id)
        {
            return _profiles.TryGetValue(id, out UserProfile profile) ? profile : null;
        }

        public UserProfile Update(UserProfile profile)
        {
            _profiles[profile.Id] = profile;
            return profile;
        }
    }
}
