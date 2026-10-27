using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class ForumService
    {
        private readonly ApplicationDbContext _forumContext;

        public ForumService(ApplicationDbContext forumContext)
        {
            _forumContext = forumContext;
        }
        public async Task<List<Forum>> GetAllAsync()
        {
           return await _forumContext.Forums.Where(i => !i.IsDeleted).ToListAsync();
        }
        public async Task<Forum?> GetAsync(int id)
        {
            return await _forumContext.Forums.FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }
    }

}
