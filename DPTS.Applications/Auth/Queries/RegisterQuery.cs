using DPTS.Applications.Auth.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Auth.Queries
{
    public class RegisterQuery : IRequest<ServiceResult<RegisterDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string PasswordComfirmed {  get; set; } = string.Empty;
        public bool IsBuyer { get; set; }
        public bool IsEnabled2FA { get; set; }
    }
}
