namespace APIDOP.Models.DB
{
    public class ForumSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
