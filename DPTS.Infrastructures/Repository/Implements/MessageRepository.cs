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

        public async Task AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Message?> GetByIdAsync(string messageId)
        {
            return await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MessageId == messageId);
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(
            string participantAId,
            ParticipantType participantAType,
            string participantBId,
            ParticipantType participantBType,
            int limit = 50,
            DateTime? before = null)
        {
            var query = _context.Messages.AsQueryable();

            query = query.Where(m =>
                (m.SenderId == participantAId && m.SenderType == participantAType &&
                 m.ReceiverId == participantBId && m.ReceiverType == participantBType)
                ||
                (m.SenderId == participantBId && m.SenderType == participantBType &&
                 m.ReceiverId == participantAId && m.ReceiverType == participantAType));

            if (before.HasValue)
                query = query.Where(m => m.CreatedAt < before.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetByReceiverAsync(
            string receiverId,
            ParticipantType receiverType,
            bool includeSystem = false)
        {
            var query = _context.Messages
                .Where(m => m.ReceiverId == receiverId && m.ReceiverType == receiverType);

            if (!includeSystem)
                query = query.Where(m => !m.IsSystem);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteAsync(string messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }

}
