using DPTS.Applications.Buyer.Dtos.profile;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.profile
{
    public  class GetUserProfileMiniQuery : IRequest<ServiceResult<UserProfileMiniDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
