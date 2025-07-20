using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Messages
{
    public interface IMessageQuery
    {
        Task<IEnumerable<Message>> GetsWithIdsJoinChat(string ownerId, ParticipantType OwnerType, string anotherId, ParticipantType anotherType);
    }
}
