using System.Dynamic;
using System.Security.Claims;
using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class ThreadEditModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly IApplicationAuthorizationService _authorizationService;

        public ThreadEditModel(DiscussionThreadService threadService, IApplicationAuthorizationService authorizationService)
        {
            _threadService = threadService;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public DiscussionEditViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var thread = await _threadService.GetAsync(id);

            if (thread == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.CanEditAsync(User, thread))
            {
                return Forbid();
            }

            Input.Title = thread.Title;
            Input.Content = thread.Content;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {


            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _threadService.GetAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.CanEditAsync(User, existing))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var thread = new DiscussionThread
            {
                Id = id,
                Title = Input.Title,
                Content = Input.Content
            };

            await _threadService.UpdateAsync(thread);
            return Redirect($"/Discussion/{thread.Id}");
        }
    }
}

