using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_category.Queries
{
    public class AddCategoryCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public AddCategory AddCategory { get; set; }
    }
    public class AddCategory
    {
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryIcon { get; set; }

    }
}
