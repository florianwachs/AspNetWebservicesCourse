using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;

namespace AspNetCoreSecurity.Domain.Data
{
    public static class KnownUsers
    {
        public struct UserCreateData
        {
            public string Id { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string UserName { get; set; }
            public bool IsProfessor { get; set; }
            public string Email { get; set; }
            public UserTypes Type { get; set; }

            public static UserCreateData New(string id, string givenName, string familyName, string email, UserTypes type)
            {
                return new UserCreateData
                {
                    Id = id,
                    GivenName = givenName,
                    FamilyName = familyName,
                    UserName = givenName + familyName,
                    Email = email,
                    Type = type
                };
            }
        }

        public static IEnumerable<UserCreateData> Get()
        {
            yield return UserCreateData.New("chuck", "Chuck", "Norris", "chuck@th.de", UserTypes.Principal);

            yield return UserCreateData.New("katie", "Katie", "Bouman", "katie@th.de", UserTypes.Professor);

            yield return UserCreateData.New("jason", "Jason", "Bourne", "jason@th.de", UserTypes.Student);
            yield return UserCreateData.New("arnold", "Arnold", "Schwarzenegger", "arnold@th.de", UserTypes.Student);
        }
    }
}
