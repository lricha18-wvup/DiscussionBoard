using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Admin
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class UsersModel : PageModel
    {
        public List<ApplicationUser> AllUsers { get; private set; } = [];
        public string? SelectedUserId { get; private set; }
        public ApplicationUser? SelectedUser { get; private set; }
        public bool SelectedUserIsAdmin { get; private set; }
        public string? SuccessMessage { get; private set; }
        public string? ErrorMessage { get; private set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationAuthorizationService _authorizationService;

        public UsersModel(UserManager<ApplicationUser> userManager, IApplicationAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task OnGetAsync(string? selectedUserId)
        {
            await LoadUsers();

            if (selectedUserId != null)
            {
                SelectedUser = await _userManager.FindByIdAsync(selectedUserId);

                if (SelectedUser != null)
                    SelectedUserIsAdmin = await _userManager.IsInRoleAsync(
                        SelectedUser, ApplicationRoles.Admin);
            }
        }

        public async Task<IActionResult> OnPostSelectUserAsync(string selectedUserId)
        {
            return RedirectToPage(new { selectedUserId });
        }

        public async Task<IActionResult> OnPostAddAsync(string selectedUserId)
        {
            var result = await _authorizationService.AddRoleAsync(
                User, selectedUserId, ApplicationRoles.Admin);

            if (result.Succeeded)
                SuccessMessage = "Admin role added.";
            else
                ErrorMessage = "Could not add admin role.";

            return RedirectToPage(new { selectedUserId });
        }

        public async Task<IActionResult> OnPostRemoveAsync(string selectedUserId)
        {
            var result = await _authorizationService.RemoveRoleAsync(
                User, selectedUserId, ApplicationRoles.Admin);

            if (result.Succeeded)
                SuccessMessage = "Admin role removed.";
            else
                ErrorMessage = "Could not remove admin role.";

            return RedirectToPage(new { selectedUserId });
        }

        private async Task LoadUsers()
        {
            AllUsers = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }
    }



}
