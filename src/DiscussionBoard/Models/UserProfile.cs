using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class UserProfile
    {
        [Required]
        [MinLength(2),MaxLength(60) ]
        public String DisplayName { get; set; }

        public String AvatarId { get; set; }

    }
}
