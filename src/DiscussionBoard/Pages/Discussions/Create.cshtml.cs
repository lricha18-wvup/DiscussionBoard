using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class Create : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int ForumId { get; set; }

        [BindProperty]
        public CreateModel Input { get; set; } = new();

        private readonly DiscussionThreadService _threadService;
        public void OnGet()
        {

        }

        public Create(DiscussionThreadService threadService)
        {
            _threadService = threadService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var thread = new DiscussionThread
            {
                Title = Input.Title,
                Content = Input.Content,
                ForumId = ForumId,
                ApplicationUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _threadService.AddAsync(thread);

            return RedirectToPage("/Index");
        }
    }
}
