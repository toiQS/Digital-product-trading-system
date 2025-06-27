using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyers.profiles.Queries
{
    public class ChangePasswordCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } = string.Empty;

        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

}
