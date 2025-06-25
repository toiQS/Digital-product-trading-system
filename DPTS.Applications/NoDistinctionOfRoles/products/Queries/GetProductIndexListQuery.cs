using DPTS.Applications.NoDistinctionOfRoles.products.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.products.Queries
{
    public class GetProductIndexListQuery : IRequest<ServiceResult<IEnumerable<ProductIndexListDto>>>
    {
        public string Text { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public int RatingOverall { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
