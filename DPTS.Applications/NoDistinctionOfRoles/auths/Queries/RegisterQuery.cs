using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Queries
{
    public class RegisterQuery : IRequest<ServiceResult<string>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordComfirm {  get; set; } = string.Empty;
        public bool IsBuyer { get; set; }

    }
}
