namespace AspNetCoreHateoasWithLinks.Models
{
    public class Issue
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public IssueStates State { get; set; }

        public static Issue CreateNew(string id)
        {
            return new Issue
            {
                Id = id,
                Name = "Dummy Task",
                Description = "Bla bla",
                UserId = "1",
                State = IssueStates.New
            };
        }
    }
}
