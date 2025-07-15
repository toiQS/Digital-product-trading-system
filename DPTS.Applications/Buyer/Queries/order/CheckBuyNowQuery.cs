using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class CheckBuyNowQuery : IRequest<ServiceResult<CheckoutDto>>
    {
        public string UserId { get; set; }= string.Empty;
        public string ProductId {  get; set; }= string.Empty;
        public int Quantity {  get; set; }
    }
}
