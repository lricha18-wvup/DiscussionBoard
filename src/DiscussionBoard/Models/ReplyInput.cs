using System.ComponentModel.DataAnnotations;

public class ReplyInput
{
    public int? ParentPostId { get; set; }

    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;
}
