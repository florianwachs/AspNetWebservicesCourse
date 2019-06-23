using System.Collections.Generic;

namespace EfCoreRelationSample.Domain
{
    public class User
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        private readonly List<SharedList> _sharedLists = new List<SharedList>();
        public IReadOnlyCollection<SharedList> SharedLists => _sharedLists.AsReadOnly();
        public static User Create(string id, string name)
        {
            return new User { Id = id, Name = name };
        }
    }
}
