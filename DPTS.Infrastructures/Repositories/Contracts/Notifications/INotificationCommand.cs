using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Notifications
{
    public interface INotificationCommand
    {
        Task addAsync(Notification notification);
    }
}
