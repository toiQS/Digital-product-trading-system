using DPTS.Applications.case_buyer.product_detail_page.dtos;
using DPTS.Applications.shareds;
using MediatR;

namespace DPTS.Applications.case_buyer.product_detail_page.models
{
    public class GetProductSuggestIndexQuery : IRequest<ServiceResult<IEnumerable<ProductSuggestIndexDto>>>
    {
        public string CategoryId { get; set; }
    }
}
