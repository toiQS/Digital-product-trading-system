using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.complaint
{
    public class ComfirmResovledComplaintCommand : IRequest<ServiceResult<string>>  
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
    }
}
