using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        public UserProfileService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
        {
            string displayNameTrimmed = displayName.Trim();
            if ((displayNameTrimmed.Length < 2) || (displayNameTrimmed.Length > 60))
            {
                return false;
            }
            string normalized = displayNameTrimmed.ToUpperInvariant();

            bool UserNameTaken =await _context.Users.AnyAsync(u =>
                u.DisplayName.ToUpper() == normalized && (excludeUserId == null || u.Id != excludeUserId));

            return !UserNameTaken;
        }
    }
}
