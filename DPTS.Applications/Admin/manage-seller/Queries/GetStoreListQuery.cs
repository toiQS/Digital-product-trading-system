using DPTS.Applications.Admin.manage_seller.dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Admin.manage_seller.Queries
{
    public class GetStoreListQuery : IRequest<ServiceResult<StoreDto>>
    {
        public string UserId { get; set; }
        public Condition Condition { get; set; }
        public int PageSize { get; set; }   
        public int PageCount { get; set; }
    }
    public record Condition
    {
        public string? Text { get; set; }
        public StoreStatus StoreStatus { get; set; }
    }
}
