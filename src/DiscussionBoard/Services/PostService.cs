using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _postContext;
        private readonly ILogger<PostService> _postLogger;

        public PostService(ApplicationDbContext postContext, ILogger<PostService> postLogger)
        {
            _postContext = postContext;
            _postLogger = postLogger;
        }

        public PostService(ApplicationDbContext postContext)
        {
            _postContext = postContext;
            _postLogger = NullLogger<PostService>.Instance;
        }

        public async Task<List<Post>> GetForThreadAsync(int threadId)
        {
            return await _postContext.Posts
                .Include(p => p.ApplicationUser)
                .Where(p => p.DiscussionThreadId == threadId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Post post)
        {
            try
            {
                await _postContext.Posts.AddAsync(post);
                await _postContext.SaveChangesAsync();
                _postLogger.LogInformation("Post {PostId} was added to thread {ThreadId}", post.Id, post.DiscussionThreadId);
            }
            catch (DbUpdateException ex)
            {
                _postLogger.LogError(ex, "Failed to add post to thread {ThreadId}", post.DiscussionThreadId);
                throw;
            }
        }

        public async Task<Post?> GetAsync(int id)
        {
            return await _postContext.Posts
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task UpdateAsync(Post post)
        {
            var existingPost = await GetAsync(post.Id);

            if (existingPost == null)
                throw new KeyNotFoundException($"Post {post.Id} was not found.");

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;

            await _postContext.SaveChangesAsync();
        }
    }
}
