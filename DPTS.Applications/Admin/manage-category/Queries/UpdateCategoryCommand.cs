using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_category.Queries
{
    public class UpdateCategoryCommand : IRequest<ServiceResult<string>>    
    {
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public UpdateCategory UpdateCategory { get; set; }
    }
    public class UpdateCategory
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool? IsFeatured { get; set; }
    }
}
