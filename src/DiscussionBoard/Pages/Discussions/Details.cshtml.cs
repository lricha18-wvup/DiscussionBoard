using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
public class Details : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly PostService _postService;

        public Details(DiscussionThreadService threadService, PostService postService)
        {
            _threadService = threadService;
            _postService = postService;
        }

        public DiscussionThread Thread { get; set; } = null!;
        public List<PostTreeBuilder.Node> PostTree { get; set; } = new();

        [BindProperty]
        public ReplyInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int threadId)
        {
            var thread = await _threadService.GetAsync(threadId);
            if (thread == null)
            {
                return NotFound();
            }

            Thread = thread;
            var posts = await _postService.GetForThreadAsync(threadId);
            PostTree = PostTreeBuilder.Build(posts);
            return Page();
        }

        public async Task<IActionResult> OnPostReplyAsync(int threadId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(threadId);
            }

            var posts = await _postService.GetForThreadAsync(threadId);
            if (Input.ParentPostId != null && !posts.Any(p => p.Id == Input.ParentPostId))
            {
                return BadRequest();
            }

            var post = new Post
            {
                Title = "Reply",
                Content = Input.Content,
                DiscussionThreadId = threadId,
                ParentPostId = Input.ParentPostId,
                ApplicationUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _postService.AddAsync(post);
            return RedirectToPage(new { threadId });
        }
    }


}

