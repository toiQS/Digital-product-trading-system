using DPTS.Applications.Seller.products.Dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.Seller.products.Queries
{
    public class SearchSellerProductsQuery : IRequest<ServiceResult<IEnumerable<ProductListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public string Text {  get; set; } = string.Empty;
        public string CategoryId {  get; set; } = string.Empty;
        public ProductStatus Status { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
