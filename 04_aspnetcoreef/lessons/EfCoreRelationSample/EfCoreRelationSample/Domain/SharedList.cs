namespace EfCoreRelationSample.Domain
{
    public class SharedList
    {
        public string UserId { get; private set; }
        public string ListId { get; private set; }
        public User User { get; private set; }
        public List List { get; private set; }

        public static SharedList CreateFor(User user, List list)
        {
            return new SharedList { ListId = list.Id, UserId = user.Id };
        }
    }
}
