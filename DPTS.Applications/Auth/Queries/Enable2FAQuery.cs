using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Auth.Queries
{
    public class Enable2FAQuery : IRequest<ServiceResult<string>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
