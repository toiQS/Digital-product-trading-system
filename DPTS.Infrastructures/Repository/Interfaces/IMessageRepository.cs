namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task DeleteAsync(string id);
        Task<Message?> GetByIdAsync(string id, bool includeSender = false, bool includeReceiver = false);
        Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id, bool includeSender = false, bool includeReceiver = false);
        Task<IEnumerable<Message>> GetsAsync(string? senderId = null, string? receiverId = null, DateTime? fromDate = null, DateTime? toDate = null, bool isSystem = false, bool includeSender = false, bool includeReceiver = false);
    }
}