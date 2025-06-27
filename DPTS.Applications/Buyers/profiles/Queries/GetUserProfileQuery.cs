using DPTS.Applications.Buyers.profiles.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyers.profiles.Queries
{
    public class GetUserProfileQuery : IRequest<ServiceResult<UserProfileDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
