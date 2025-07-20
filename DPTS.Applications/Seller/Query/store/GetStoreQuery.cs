using DPTS.Applications.Seller.Dtos.store;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.store
{
    public class GetStoreQuery : IRequest<ServiceResult<DetailStoreDto>>
    {
        public string SellerId { get; set; }
    }
}
