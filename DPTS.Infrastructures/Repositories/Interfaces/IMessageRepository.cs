using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        #region Query

        Task<List<Message>> GetConversationAsync(
            string participantAId,
            ParticipantType participantAType,
            string participantBId,
            ParticipantType participantBType,
            int limit = 50);

        Task<List<Message>> GetRecentMessagesAsync(string userIdOrStoreId, ParticipantType type, int limit = 10);

        Task<List<Message>> GetUnreadMessagesAsync(string receiverId, ParticipantType receiverType);

        Task<int> CountUnreadMessagesAsync(string receiverId, ParticipantType receiverType);

        #endregion

        #region Crud

        Task AddAsync(Message message);

        #endregion
    }
}
