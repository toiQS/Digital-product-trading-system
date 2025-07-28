using DPTS.Applications.Admin.manage_category.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_category.Queries
{
    public class GetCategoryQuery : IRequest<ServiceResult<CategoryDto>>
    {
        public string UserId { get; set; }
        public Condition Condition { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public MethodSord MethodSord { get; set; }
    }
    public class Condition
    {
        public string Text { get; set; }
        public bool? IsFeatured { get; set; }
    }
    public enum MethodSord
    {
        None,
        Sord,
        SordDescending
    }
}
