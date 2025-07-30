using DPTS.Applications.Admin.manage_user.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_user.Queries
{
    public  class GetUsersQuery : IRequest<ServiceResult<UserDto>>
    {
        public string UserId { get; set; }
        public Condition Condition { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
    public class Condition
    {
        public string? Text { get; set; }
        public bool? IsAvalible { get; set; } = null;
    }
}
