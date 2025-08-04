using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task AddRangeAsync(IEnumerable<Message> messages);
        Task<IEnumerable<Message>> GetAllByParticipantAsync(ParticipantType type, string participantId);
        Task<IEnumerable<Message>> GetConversationAsync(ParticipantType participantAType, string participantAId, ParticipantType participantBType, string participantBId);
        Task<IEnumerable<Message>> GetSystemMessagesAsync(ParticipantType receiverType, string receiverId, DateTime? from = null, DateTime? to = null);
    }
}