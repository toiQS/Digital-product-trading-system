using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class GetProductIndexListQuery : IRequest<ServiceResult<ProductIndexListDto>>
    {
        public string Text { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public int RatingOverall { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
