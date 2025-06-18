using DPTS.Domains;
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
            string? orderId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeSender = false,
            bool includeReceiver = false,
            bool includeOrder = false)
        {
            var query = _context.Messages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(senderId))
                query = query.Where(m => m.SenderId == senderId);

            if (!string.IsNullOrWhiteSpace(receiverId))
                query = query.Where(m => m.ReceiverId == receiverId);

            if (!string.IsNullOrWhiteSpace(orderId))
                query = query.Where(m => m.OrderId == orderId);

            if (fromDate != null)
                query = query.Where(m => m.CreatedAt >= fromDate);

            if (toDate != null)
                query = query.Where(m => m.CreatedAt <= toDate);

            if (includeSender)
                query = query.Include(m => m.Sender);

            if (includeReceiver)
                query = query.Include(m => m.Receiver);

            if (includeOrder)
                query = query.Include(m => m.Order);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id, bool includeSender = false, bool includeReceiver = false)
        {
            var query = _context.Messages
                .Where(m =>
                    m.SenderId == user1Id && m.ReceiverId == user2Id ||
                    m.SenderId == user2Id && m.ReceiverId == user1Id)
                .AsQueryable();

            if (includeSender)
                query = query.Include(m => m.Sender);

            if (includeReceiver)
                query = query.Include(m => m.Receiver);

            return await query
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(string id, bool includeSender = false, bool includeReceiver = false, bool includeOrder = false)
        {
            var query = _context.Messages.Where(m => m.MessageId == id);

            if (includeSender)
                query = query.Include(m => m.Sender);

            if (includeReceiver)
                query = query.Include(m => m.Receiver);

            if (includeOrder)
                query = query.Include(m => m.Order);

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
