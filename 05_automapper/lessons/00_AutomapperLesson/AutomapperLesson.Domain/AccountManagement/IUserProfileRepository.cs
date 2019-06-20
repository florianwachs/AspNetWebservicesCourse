using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomapperLesson.Domain.AccountManagement
{
    public interface IUserProfileRepository
    {
        IReadOnlyCollection<UserProfile> All();
        UserProfile Update(UserProfile profile);
    }
}
