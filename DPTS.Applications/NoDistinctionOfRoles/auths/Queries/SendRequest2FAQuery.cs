using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Queries
{
    public class SendRequest2FAQuery : IRequest<ServiceResult<string>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
