using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class AddProductCommand : IRequest<ServiceResult<string>>
    {
        public string SellerId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; } 
        public string SumaryFeature { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
    }
}
