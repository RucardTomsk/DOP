namespace APIDOP.Models.DB
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
