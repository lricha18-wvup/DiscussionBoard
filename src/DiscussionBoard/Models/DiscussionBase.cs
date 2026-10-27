using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class DiscussionBase : EntityBase
    {

        [Required]
        [MinLength(3),MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(3), MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
