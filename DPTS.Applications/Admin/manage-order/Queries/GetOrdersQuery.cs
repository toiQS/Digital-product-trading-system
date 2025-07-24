using DPTS.Applications.Admin.manage_order.dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Admin.manage_order.Queries
{
    public class GetOrdersQuery : IRequest<ServiceResult<OrderDto>>
    {
        public string UserId { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public Condition Condition { get; set; }
    }
    public class Condition
    {
        public string Text { get; set; }
        public EscrowStatus EscrowStatus { get; set; }
        public RangeTime RangeTime { get; set; }
    }
    public enum RangeTime
    {
        None = 0,
        Today,
        Week,
        Month,
        Year
    }
}
