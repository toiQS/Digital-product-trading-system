using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Messages
{
    public interface IMessageQuery
    {
        Task<IEnumerable<Message>> GetsWithIdsJoinChat(string personFirstId, ParticipantType PersonFirstType, string personSecondId, ParticipantType personSecondType);
    }
}
