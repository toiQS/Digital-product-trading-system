using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Admin.manage_seller.Queries
{
    public class UpdateStatusStoreCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public StoreStatus StoreStatus { get; set; }
    }
}
