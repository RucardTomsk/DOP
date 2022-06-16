namespace APIDOP.Models.DB
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public ForumSection ForumSection { get; set; }
        public int ForumSectionId { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
