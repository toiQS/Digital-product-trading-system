using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Notifications
{
    public interface INotificationCommand
    {
        Task AddAsync(Notification notification);

        Task MarkAsReadAsync(string notificationId);

        Task MarkAllAsReadAsync(string receiverId, ReceiverType type);
    }
}
