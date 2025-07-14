using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        #region Query

        Task<List<Notification>> GetByReceiverAsync(string receiverId, ReceiverType type, int limit = 20);

        Task<int> CountUnreadAsync(string receiverId, ReceiverType type);

        Task<List<Notification>> GetUnreadAsync(string receiverId, ReceiverType type, int limit = 10);

        Task<Notification?> GetByIdAsync(string notificationId);

        #endregion

        #region Crud

        Task AddAsync(Notification notification);

        Task MarkAsReadAsync(string notificationId);

        Task MarkAllAsReadAsync(string receiverId, ReceiverType type);

        #endregion
    }
}
