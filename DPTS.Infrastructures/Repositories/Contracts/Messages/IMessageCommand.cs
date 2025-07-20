using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Messages
{
    public interface IMessageCommand
    {
        Task AddAsync(Message message);
    }
}
