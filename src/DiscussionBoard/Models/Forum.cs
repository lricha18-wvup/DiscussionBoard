using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class Forum : EntityBase
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
    }
}
