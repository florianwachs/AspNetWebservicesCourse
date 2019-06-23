using System.Collections.Generic;

namespace EfCoreRelationSample.Domain
{
    public class List
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly List<SharedList> _sharedWith = new List<SharedList>();
        public IReadOnlyCollection<SharedList> SharedWith => _sharedWith.AsReadOnly();

        public static List Create(string id, string name)
        {
            return new List { Id = id, Name = name };
        }

        public void ShareWithUser(User user)
        {
            _sharedWith.Add(SharedList.CreateFor(user, this));
        }
    }
}
