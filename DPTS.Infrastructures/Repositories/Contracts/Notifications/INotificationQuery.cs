using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Notifications
{
    public interface INotificationQuery
    {
        Task<List<Notification>> GetByReceiverAsync(string receiverId, ReceiverType type, int limit = 20);

        Task<int> CountUnreadAsync(string receiverId, ReceiverType type);

        Task<List<Notification>> GetUnreadAsync(string receiverId, ReceiverType type, int limit = 10);

        Task<Notification?> GetByIdAsync(string notificationId);
    }
}
