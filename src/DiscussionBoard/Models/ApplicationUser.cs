

using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public String DisplayName { get; set; } = string.Empty;
        public String AvatarId { get; set; } = "default";
    }
}
