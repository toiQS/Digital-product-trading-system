using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class UpdateUserProfileMiniCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ImageUser {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
    }
}
