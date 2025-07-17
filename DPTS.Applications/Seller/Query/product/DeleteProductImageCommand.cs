using DPTS.Applications.Shareds;
using MediatR;
using Org.BouncyCastle.Bcpg.Sig;

namespace DPTS.Applications.Seller.Query.product
{
    public class DeleteProductImageCommand : IRequest<ServiceResult<string>>
    {
        public string SellerId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
    }
}
