using System.Security.Claims;
using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Services
{
    public class ApplicationAuthorizationService : IApplicationAuthorizationService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        private string ErrorMessage { get; set; } = string.Empty;

        public ApplicationAuthorizationService(IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public async Task<bool> CanEditAsync(ClaimsPrincipal principal, DiscussionBase resource)
        {
            var result = await _authorizationService.AuthorizeAsync(principal, resource, ApplicationAuthorizationPolicies.IsOwnerOrAdmin);
            return result.Succeeded;
        }

        public async Task<IdentityResult> AddRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            if (actingUser.Identity?.IsAuthenticated != true || !actingUser.IsInRole(ApplicationRoles.Admin))
            {
                ErrorMessage = "Not authorized.";
                return Failed();
            }

            if (roleName != ApplicationRoles.Admin)
            {
                ErrorMessage = "Unsupported role.";
                return Failed();
            }

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null)
            {
                ErrorMessage = "User not found.";
                return Failed();
            }

            if (user.Id == _userManager.GetUserId(actingUser))
            {
                ErrorMessage = "You cannot grant yourself the Admin role.";
                return Failed();
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            if (actingUser.Identity?.IsAuthenticated != true || !actingUser.IsInRole(ApplicationRoles.Admin))
            {
                ErrorMessage = "Not authorized.";
                return Failed();
            }

            if (roleName != ApplicationRoles.Admin)
            {
                ErrorMessage = "Unsupported role.";
                return Failed();
            }

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null)
            {
                ErrorMessage = "User not found.";
                return Failed();
            }

            if (user.Id == _userManager.GetUserId(actingUser))
            {
                ErrorMessage = "You cannot remove your own Admin role.";
                return Failed();
            }

            var admins = await _userManager.GetUsersInRoleAsync(ApplicationRoles.Admin);
            if (admins.Count <= 1)
            {
                ErrorMessage = "The last remaining Admin cannot be removed.";
                return Failed();
            }

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        private IdentityResult Failed()
        {
            return IdentityResult.Failed(new IdentityError { Description = ErrorMessage });
        }
    }



}
