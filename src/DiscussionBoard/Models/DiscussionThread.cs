namespace DiscussionBoard.Models
{
    public class DiscussionThread : DiscussionBase
    {
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public Forum Forum { get; set; }
        public int ForumId { get; set; }
    }
}
