using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Handler.product
{
    public class AddProductAdjustmentCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }

    }
    public class Condition
    {

    }
}
