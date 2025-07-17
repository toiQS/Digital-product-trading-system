using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class UpdateProductImageCommand : IRequest<ServiceResult<string>>
    {
        public string SellerId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
        public string NewImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
