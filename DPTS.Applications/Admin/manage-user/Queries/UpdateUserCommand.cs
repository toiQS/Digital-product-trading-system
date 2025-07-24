using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_user.Queries
{
    public class UpdateUserCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public UpdateUser UpdateUser { get; set; }
    }
    public class UpdateUser
    {
        public string UserId { get; set; }
        public bool IsAvalible { get; set; }
    }
}
