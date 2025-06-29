using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task AddRangeAsync(IEnumerable<Message> messages);
        Task<IEnumerable<Message>> GetAllByParticipantAsync(ParticipantType type, string participantId, int skip = 0, int take = 50);
        Task<IEnumerable<Message>> GetConversationAsync(ParticipantType participantAType, string participantAId, ParticipantType participantBType, string participantBId, int skip = 0, int take = 50);
        Task<IEnumerable<Message>> GetSystemMessagesAsync(ParticipantType receiverType, string receiverId, DateTime? from = null, DateTime? to = null);
    }
}