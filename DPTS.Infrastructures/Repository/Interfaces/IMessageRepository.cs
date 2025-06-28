namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<Message?> GetByIdAsync(string messageId);
        Task<IEnumerable<Message>> GetConversationAsync(
            string participantAId,
            ParticipantType participantAType,
            string participantBId,
            ParticipantType participantBType,
            int limit = 50,
            DateTime? before = null);

        Task<IEnumerable<Message>> GetByReceiverAsync(
            string receiverId,
            ParticipantType receiverType,
            bool includeSystem = false);

        Task DeleteAsync(string messageId);
    }

}