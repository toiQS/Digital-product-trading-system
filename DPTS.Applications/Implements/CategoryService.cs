using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    /// <summary>
    /// Service xử lý các thao tác liên quan đến danh mục (Category)
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách danh mục có phân trang, bao gồm tổng số sản phẩm trong từng danh mục
        /// </summary>
        /// <returns>Danh sách danh mục dạng DTO kèm số lượng sản phẩm</returns>
        public async Task<ServiceResult<IEnumerable<CategoryIndexDto>>> GetCategories(int pageSize = 10, int pageNumber = 2)
        {
            _logger.LogInformation("Fetching paginated list of categories...");

            var categories = await _context.Categories
                .OrderBy(c => c.CreateAt) 
                .Include(c => c.Products) 
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .Select(c => new CategoryIndexDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryIcon = c.CategoryIcon,
                    Quantity = c.Products.Count 
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<CategoryIndexDto>>.Success(categories);
        }
    }
}
