using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    [Authorize]
    public class MyProfile : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _profileService;
        public string? SuccessMessage { get; set; }

        public MyProfile(UserManager<ApplicationUser> userManager, IUserProfileService profileService)
        {
            _userManager = userManager;
            _profileService = profileService;
        }

        [BindProperty]
        public UserProfile Input { get; set; } = new UserProfile();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            Input = new UserProfile
            {
                DisplayName = user.DisplayName,
                AvatarId = user.AvatarId
            };

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            Input.DisplayName = (Input.DisplayName != null ? Input.DisplayName : string.Empty).Trim();

            if (Input.DisplayName.Length < 2 || Input.DisplayName.Length > 60)
            {
                ModelState.AddModelError("Input.DisplayName", "Display name must be 2-60 characters.");
            }
            else if (!await _profileService.IsDisplayNameAvailableAsync(Input.DisplayName, user.Id))
            {
                ModelState.AddModelError("Input.DisplayName", "That display name is already taken.");
            }

            if (!AvatarCatalog.IsValid(Input.AvatarId))
            {
                ModelState.AddModelError("Input.AvatarId", "Please choose a valid avatar.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.DisplayName = Input.DisplayName;
            user.AvatarId = Input.AvatarId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            SuccessMessage = "Your profile was updated.";
            return Page();
        }
    }
}
