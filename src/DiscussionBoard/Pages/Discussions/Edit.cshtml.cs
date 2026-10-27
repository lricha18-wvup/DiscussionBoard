using System.Dynamic;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class Edit : PageModel
    {
        private readonly DiscussionThreadService _threadService;

        public Edit(DiscussionThreadService threadService)
        {
            _threadService = threadService;
        }

        [BindProperty]
        public EditModel Input { get; set; } = new();

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var thread = await _threadService.GetAsync(id);

            if (thread == null || thread.ApplicationUserId != userId)
            {
                return NotFound();
            }

            Input.Title = thread.Title;
            Input.Content = thread.Content;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _threadService.GetAsync(id);

            if (existing == null || existing.ApplicationUserId != userId)
            {
                return NotFound();
            }

            var thread = new DiscussionThread
            {
                Id = id,
                Title = Input.Title,
                Content = Input.Content
            };

            await _threadService.UpdateAsync(thread);
            return RedirectToPage("/Index");
        }
    }
}

