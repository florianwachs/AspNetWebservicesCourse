namespace AutomapperLesson.Domain.AccountManagement
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool IsAdmin { get; set; }
        public string CreditCardNumber { get; set; }
    }
}
