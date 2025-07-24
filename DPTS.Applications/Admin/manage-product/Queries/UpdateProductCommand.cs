using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Admin.manage_product.Queries
{
    public class UpdateProductCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public ConditionUpdateProduct ConditionUpdateProduct { get; set; }
    }
    public class ConditionUpdateProduct
    {
        public string ProductId { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
}
