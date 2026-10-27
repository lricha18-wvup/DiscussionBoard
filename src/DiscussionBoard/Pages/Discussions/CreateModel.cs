using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Pages.Discussions
{
    public class CreateModel
    {
        [Required]
        [MinLength(3),MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MinLength(3), MaxLength(1000)]
        public string Content { get; set; }
    }
}
