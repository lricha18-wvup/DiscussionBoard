using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Forums
{
    public class Details : PageModel
    {
        private readonly ForumService _forumService;
        private readonly DiscussionThreadService _threadService;

        public Details(ForumService forumService, DiscussionThreadService threadService)
        {
            _forumService = forumService;
            _threadService = threadService;
        }

        public Forum Forum { get; set; } = null!;
        public List<DiscussionThread> Threads { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var forum = await _forumService.GetAsync(id);
            if (forum == null)
            {
                return NotFound();
            }

            Forum = forum;
            Threads = await _threadService.GetForForumAsync(id);
            return Page();
        }
    }
}
