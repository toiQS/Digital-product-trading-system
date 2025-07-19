using DPTS.Applications.case_buyer.product_detail_page.dtos;
using DPTS.Applications.shareds;
using MediatR;

namespace DPTS.Applications.case_buyer.product_detail_page.models
{
    public class GetProductDetailQuery : IRequest<ServiceResult<ProductDetailDto>>
    {
        public string ProductId { get; set; }
    }
}
