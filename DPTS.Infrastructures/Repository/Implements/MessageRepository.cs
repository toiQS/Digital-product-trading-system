using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetsAsync(
            string? senderId = null,
            string? receiverId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool isSystem = false,
            bool includeSender = false,
            bool includeReceiver = false)
        {
            var query = _context.Messages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(senderId))
            {
                query = query.Where(m =>
                    m.SenderUserId == senderId ||
                    m.SenderStoreId == senderId);
            }

            if (!string.IsNullOrWhiteSpace(receiverId))
            {
                query = query.Where(m =>
                    m.ReceiverUserId == receiverId ||
                    m.ReceiverStoreId == receiverId);
            }

            if (isSystem)
            {
                query = query.Where(m => m.IsSystem);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt <= toDate.Value);
            }

            if (includeSender)
            {
                query = query
                    .Include(m => m.SenderUser)
                    .Include(m => m.SenderStore);
            }

            if (includeReceiver)
            {
                query = query
                    .Include(m => m.ReceiverUser)
                    .Include(m => m.ReceiverStore);
            }

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(
            string user1Id,
            string user2Id,
            bool includeSender = false,
            bool includeReceiver = false)
        {
            var query = _context.Messages
                .Where(m =>
                    ((m.SenderUserId == user1Id || m.SenderStoreId == user1Id) &&
                     (m.ReceiverUserId == user2Id || m.ReceiverStoreId == user2Id)) ||

                    ((m.SenderUserId == user2Id || m.SenderStoreId == user2Id) &&
                     (m.ReceiverUserId == user1Id || m.ReceiverStoreId == user1Id)))
                .AsQueryable();

            if (includeSender)
            {
                query = query
                    .Include(m => m.SenderUser)
                    .Include(m => m.SenderStore);
            }

            if (includeReceiver)
            {
                query = query
                    .Include(m => m.ReceiverUser)
                    .Include(m => m.ReceiverStore);
            }

            return await query
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(
            string id,
            bool includeSender = false,
            bool includeReceiver = false)
        {
            var query = _context.Messages
                .Where(m => m.MessageId == id)
                .AsQueryable();

            if (includeSender)
            {
                query = query
                    .Include(m => m.SenderUser)
                    .Include(m => m.SenderStore);
            }

            if (includeReceiver)
            {
                query = query
                    .Include(m => m.ReceiverUser)
                    .Include(m => m.ReceiverStore);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
