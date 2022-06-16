namespace APIDOP.Models.DB
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
        public Message Message { get; set; }

        public int MessageId { get; set; }
    }
}
