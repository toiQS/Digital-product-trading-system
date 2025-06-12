using MediatR;

namespace DPTS.Applications.Seller.products.Queries
{
    public class GetProductsItemQuery : IRequest<GetProductsItemQuery>
    {
        public string SellerId { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
