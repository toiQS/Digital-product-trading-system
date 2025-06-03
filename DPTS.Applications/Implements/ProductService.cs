using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products with page size: {pageSize} and page number: {pageNumber}");
            var products = await GetBaseProductsQueryable()
                .Where(p => p.IsCirculate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await BuildProductIndexResult(products);
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBySellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products for seller with seller id: {sellerId} page size: {pageSize} and page number: {pageNumber}");
            var products = await GetBaseProductsQueryable()
                .Where(p => p.SellerId == sellerId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await BuildProductIndexResult(products);
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleAsync(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products is the best sale with page size: {pageSize} and page number: {pageNumber}");
            var products = await GetBaseProductsQueryable()
                .Where(p => p.IsCirculate)
                .ToListAsync();

            var result = await BuildProductIndexResult(products);
            var sorted = result.Data
                .OrderByDescending(x => x.QuantitySelled)
                .Take(pageSize);

            return ServiceResult<IEnumerable<ProductIndexDto>>.Success(sorted);
        }

        public async Task<ServiceResult<ProductDetailDto>> GetProductByIdAsync(string productId)
        {
            _logger.LogInformation($"Fetching product with product id: {productId}");
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews).ThenInclude(r => r.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return ServiceResult<ProductDetailDto>.Error("Product not found");

            var productDetail = new ProductDetailDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                CategoryName = product.Category.CategoryName,
                Rating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                Status = EnumHandle.HandleProductStatus(product.Status),
                Images = product.Images.Select(img => new ImageIndexDto
                {
                    ImageId = img.ImageId,
                    ImagePath = img.ImagePath
                }).ToList(),
                Reviews = product.Reviews.Select(r => new ProductReviewIndexDto
                {
                    ProductReviewId = r.ProductId,
                    BuyerName = r.User.FullName ?? "N/A",
                    Content = r.Comment,
                    Likes = r.Likes,
                    Rating = r.Rating
                }).ToList()
            };

            return ServiceResult<ProductDetailDto>.Success(productDetail);
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsByCategoryIdAndRating(string categoryId, int rating, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products with category id: {categoryId}, rating: {rating}, page size: {pageSize} and page number: {pageNumber}");
            var products = await GetBaseProductsQueryable()
                .Where(p => p.IsCirculate && p.CategoryId == categoryId)
                .ToListAsync();

            var allProducts = await BuildProductIndexResult(products);
            var filtered = allProducts.Data.Where(x => x.Rating >= rating)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize);

            return ServiceResult<IEnumerable<ProductIndexDto>>.Success(filtered);
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> CanBeLikedAsync(string categoryId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products can be liked with category id: {categoryId}, page size: {pageSize} and page number: {pageNumber}");
            var products = await GetBaseProductsQueryable()
                .Where(p => p.IsCirculate && p.CategoryId == categoryId)
                .ToListAsync();

            var result = await BuildProductIndexResult(products);
            var sorted = result.Data
                .OrderByDescending(x => x.Rating)
                .Take(pageSize);

            return ServiceResult<IEnumerable<ProductIndexDto>>.Success(sorted);
        }


        private IQueryable<Product> GetBaseProductsQueryable()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews);
        }

        private async Task<ServiceResult<IEnumerable<ProductIndexDto>>> BuildProductIndexResult(List<Product> products)
        {
            var productIds = products.Select(p => p.ProductId).ToList();

            var orderItems = await _context.OrderItems
                .Where(oi => productIds.Contains(oi.ProductId) &&
                             oi.Order.Escrow.Status == EscrowStatus.Done)
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Escrow)
                .ToListAsync();

            var groupedOrderItems = orderItems.GroupBy(oi => oi.ProductId)
                                              .ToDictionary(g => g.Key, g => g.ToList());

            var productDtos = products.Select(product => new ProductIndexDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Category = product.Category.CategoryName,
                Image = product.Images.OrderBy(i => i.CreateAt).LastOrDefault()?.ImagePath ?? "default.png",
                Price = product.Price,
                QuantitySelled = groupedOrderItems.ContainsKey(product.ProductId)
                    ? groupedOrderItems[product.ProductId].Sum(x => x.Quantity)
                    : 0,
                Rating = product.Reviews.Any() ? product.Reviews.Average(x => x.Rating) : 0,
                Status = EnumHandle.HandleProductStatus(product.Status)
            }).ToList();

            return ServiceResult<IEnumerable<ProductIndexDto>>.Success(productDtos);
        }
    }
}
