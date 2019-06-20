namespace AspNetCoreHateoasWithLinks.Models
{
    public enum TaskStates
    {
        New,
        InProgress,
        Done,
    }

    public class TaskDto : LinkedResourceBaseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public TaskStates TaskState { get; set; }

        public static TaskDto CreateNew(string id)
        {
            return new TaskDto
            {
                Id = id,
                Name = "Dummy Task",
                Description = "Bla bla",
                UserId = "1",
                TaskState = TaskStates.New
            };
        }
    }
}
