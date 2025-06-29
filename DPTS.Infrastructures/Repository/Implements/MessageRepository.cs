using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class MessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create

        public async Task AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Message> messages)
        {
            _context.Messages.AddRange(messages);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<IEnumerable<Message>> GetConversationAsync(
            ParticipantType participantAType,
            string participantAId,
            ParticipantType participantBType,
            string participantBId,
            int skip = 0,
            int take = 50)
        {
            var query = _context.Messages.Where(m =>
                (m.SenderType == participantAType && m.SenderId == participantAId &&
                 m.ReceiverType == participantBType && m.ReceiverId == participantBId)
                ||
                (m.SenderType == participantBType && m.SenderId == participantBId &&
                 m.ReceiverType == participantAType && m.ReceiverId == participantAId)
            );

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetAllByParticipantAsync(
            ParticipantType type,
            string participantId,
            int skip = 0,
            int take = 50)
        {
            return await _context.Messages
                .Where(m => (m.SenderType == type && m.SenderId == participantId) ||
                            (m.ReceiverType == type && m.ReceiverId == participantId))
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetSystemMessagesAsync(
            ParticipantType receiverType,
            string receiverId,
            DateTime? from = null,
            DateTime? to = null)
        {
            var query = _context.Messages
                .Where(m => m.IsSystem && m.ReceiverType == receiverType && m.ReceiverId == receiverId);

            if (from.HasValue)
                query = query.Where(m => m.CreatedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(m => m.CreatedAt <= to.Value);

            return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
        }

        #endregion
    }
}
