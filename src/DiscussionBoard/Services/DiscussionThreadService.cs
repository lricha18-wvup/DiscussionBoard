using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services
{
    public class DiscussionThreadService
    {
        private readonly ApplicationDbContext _discussionThreadContext;
        private readonly ILogger <DiscussionThreadService> _discussionThreadLogger;

        public DiscussionThreadService(ApplicationDbContext discussionThreadContext, ILogger <DiscussionThreadService> discussionThreadLogger)
        {
            _discussionThreadContext = discussionThreadContext;
            _discussionThreadLogger = discussionThreadLogger;
        }

        //From tests looks like it is only supposed to take a single parameter
        public DiscussionThreadService(ApplicationDbContext discussionThreadContext)
        {
            _discussionThreadContext = discussionThreadContext;
            _discussionThreadLogger = NullLogger<DiscussionThreadService>.Instance;
        }

        public async Task<DiscussionThread?> GetAsync(int id)
        {
            return await _discussionThreadContext.DiscussionThreads
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
        {
            return await _discussionThreadContext.DiscussionThreads.Include(t => t.ApplicationUser).Where(d => d.ForumId == forumId && !d.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(DiscussionThread thread)
        {
            try
            {
                await _discussionThreadContext.DiscussionThreads.AddAsync(thread);
                await _discussionThreadContext.SaveChangesAsync();
                _discussionThreadLogger.LogInformation("Thread {Title} (Id {ThreadId}) was added", thread.Title, thread.Id);
            }
            catch (DbUpdateException ex)
            {
                _discussionThreadLogger.LogError(ex, "Failed to add thread {Title} to forum {ForumId}", thread.Title, thread.ForumId);
                throw;
            }

        }

        public async Task UpdateAsync(DiscussionThread thread)
        {
            try
            {
                var existingThread = await GetAsync(thread.Id);

                if (existingThread == null)
                {
                    _discussionThreadLogger.LogWarning("Thread {ThreadId} was not found or is deleted, so it was not updated", thread.Id);
                    throw new KeyNotFoundException($"Thread {thread.Id} was not found.");
                }

                existingThread.Title = thread.Title;
                existingThread.Content = thread.Content;

                await _discussionThreadContext.SaveChangesAsync();
                _discussionThreadLogger.LogInformation("Thread {Title} (Id {ThreadId}) was updated", existingThread.Title, existingThread.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _discussionThreadLogger.LogError(ex, "Thread {ThreadId} could not be updated because it was changed or deleted by someone else", thread.Id);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _discussionThreadLogger.LogError(ex, "Failed to update thread {Title} (Id {ThreadId})", thread.Title, thread.Id);
                throw;
            }
        }
    }
}
