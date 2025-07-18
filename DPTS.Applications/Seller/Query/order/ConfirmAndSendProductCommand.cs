using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.order
{
    public class ConfirmAndSendProductCommand : IRequest<ServiceResult<string>>
    {
        public string SellerId { get; set; }
        public string EscrowId { get; set; }
        public string StoreId { get; set; }
    }
}
