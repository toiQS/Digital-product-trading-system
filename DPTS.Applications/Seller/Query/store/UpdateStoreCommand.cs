using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.store
{
    public class UpdateStoreCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public UpdateStore UpdateStore { get; set; }
    }

    public class UpdateStore
    {
        public string Name { get; set; }
        public string Image {  get; set; }
    }
}
