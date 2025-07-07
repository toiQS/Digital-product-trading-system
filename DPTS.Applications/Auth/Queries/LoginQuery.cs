using DPTS.Applications.Auth.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Auth.Queries
{
    public class LoginQuery : IRequest<ServiceResult<LoginDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
