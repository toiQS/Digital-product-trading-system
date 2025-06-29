using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Auth.Queries
{
    public class Comfirm2FAQuery : IRequest<ServiceResult<string>>
    {
        public string Email {  get; set; } = string.Empty;
        public string SecretCode { get; set; } = string.Empty;
    }
}
