using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.APIs.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories([FromQuery] PagingModel pagingModel)
        {
            // Default fallback if values not provided
            var pageSize = pagingModel.PageSize > 0 ? pagingModel.PageSize : 10;
            var pageNumber = pagingModel.PageNumber > 0 ? pagingModel.PageNumber : 1;

            var result = await _categoryService.GetCategories(pageSize, pageNumber);

            return result.Status switch
            {
                StatusResult.Success => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Warning => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Failed => NotFound(result.MessageResult),
                StatusResult.Errored => StatusCode(500, result.MessageResult),
                _ => BadRequest("Trạng thái không xác định.")
            };
        }
    }
}
