using DPTS.Applications.Seller.Dtos.store;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.store
{
    public class GetAdjustmentRuleInStoreQuery : IRequest<ServiceResult<AdjustmentRuleListInStoreDto>>
    {
        public string StoreId { get; set; } = string.Empty;
        public int PageIndex { get; set; } 
        public int PageSize { get; set; }
    }
}
