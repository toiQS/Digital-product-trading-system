using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.product
{
    public class GetProductIndexListQuery : IRequest<ServiceResult<ProductIndexListDto>>
    {
        
        public Condition Condition { get; set; } = new Condition();
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
    public class Condition
    {
        public string? Text { get; set; } = string.Empty;
        public int RatingOverall { get; set; }
        public List<string> CategoryIds { get; set; } = new List<string>();
    }
}
