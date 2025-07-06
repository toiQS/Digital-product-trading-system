using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public  class GetUserProfileMiniQuery : IRequest<ServiceResult<UserProfileMiniDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
