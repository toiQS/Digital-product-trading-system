using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

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

        #region Pulic Methods
        #region Buyer
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products with page size:{pageSize} and page number {pageNumber}.");
            try
            {
                var products = await GetProductsAsync();
                return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products), pageNumber, pageSize));
            }
            catch
            {
                _logger.LogError("Error when fecthing products");
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Lỗi khi thực hiện trả về danh sách sản phẩm");
            }
        }
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleAsync(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products sort with product is best sold with page number: {pageNumber} and page size: {pageSize}.");
            try
            {
                var products = await GetProductsAsync();
                return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products).OrderByDescending(x => x.QuantitySelled), pageNumber, pageSize));
            }
            catch
            {
                _logger.LogError("Error when fetching products best sale.");
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Lỗi khi thực hiện trả về danh sách sản phẩm bán chạy nhất");
            }
        }
        #endregion
        #region  Seller
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBySellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products by seller id: {sellerId} with page number: {pageNumber} and page size: {pageSize}.");
            try
            {
                var products = await GetProductsAsync();
                
                return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.SellerId == sellerId)), pageNumber, pageSize));
            }
            catch
            {
                _logger.LogError($"Error when fetching products by seller id: {sellerId}.");
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Lỗi khi thực hiện trả về danh sách sản phẩm của người bán");
            }
        }
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleWithSellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products best sale by seller id: {sellerId} with page number: {pageNumber} and page size: {pageSize}.");
            try
            {
                var products = await GetProductsAsync(); 
                return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.SellerId == sellerId)).OrderByDescending(x => x.QuantitySelled), pageNumber, pageSize));
            }
            catch
            {
                _logger.LogError($"Error when fetching products best sale by seller id: {sellerId}.");
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Lỗi khi thực hiện trả về danh sách sản phẩm bán chạy nhất của người bán");
            }
        }
        #endregion
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsWithManyOptions(string text, string categoryId, int rating, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching products with text: {text}, category id: {categoryId}, rating: {rating}, page number: {pageNumber} and page size: {pageSize}.");
            try
            {
                var products = await GetProductsAsync();
                switch (products)
                {
                    case var _ when string.IsNullOrEmpty(text) && string.IsNullOrEmpty(categoryId) && rating <= 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products), pageNumber, pageSize));
                    case var _ when !string.IsNullOrEmpty(text) && string.IsNullOrEmpty(categoryId) && rating <= 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.ProductName.Contains(text))), pageNumber, pageSize));
                    case var _ when string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(categoryId) && rating <= 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.CategoryId == categoryId)), pageNumber, pageSize));
                    case var _ when string.IsNullOrEmpty(text) && string.IsNullOrEmpty(categoryId) && rating > 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.Reviews.Any(r => r.Rating >= rating))), pageNumber, pageSize));
                    case var _ when !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(categoryId) && rating <= 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.ProductName.Contains(text) && p.CategoryId == categoryId)), pageNumber, pageSize));
                    case var _ when !string.IsNullOrEmpty(text) && string.IsNullOrEmpty(categoryId) && rating > 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.ProductName.Contains(text) && p.Reviews.Any(r => r.Rating >= rating))), pageNumber, pageSize));
                    case var _ when string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(categoryId) && rating > 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.CategoryId == categoryId && p.Reviews.Any(r => r.Rating >= rating))), pageNumber, pageSize));
                    case var _ when !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(categoryId) && rating > 0:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products.Where(p => p.ProductName.Contains(text) && p.CategoryId == categoryId && p.Reviews.Any(r => r.Rating >= rating))), pageNumber, pageSize));
                    default:
                        return ServiceResult<IEnumerable<ProductIndexDto>>.Success(PagingAsync(MapToProductIndexDtos(products), pageNumber, pageSize));
                }
            }
            catch
            {
                _logger.LogError("Error when fetching products with many options.");
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Lỗi khi thực hiện trả về danh sách sản phẩm với nhiều tùy chọn");
            }
        }


        public async Task<ServiceResult<ProductDetailDto>> GetProductByIdAsync(string productId)
        {
            _logger.LogInformation(productId, "Fetching product by id: {productId}.");
            try
            {
                var product = await _context.Products
                    .Include(p => p.Seller)
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Include(p => p.Reviews)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
               var rating = product.Reviews.Select(x => x.Rating).Average();
                if (product == null)
                {
                    _logger.LogWarning($"Product with id: {productId} not found.");
                    return ServiceResult<ProductDetailDto>.Error("Sản phẩm không tồn tại");
                }
                var productDetailDto = new ProductDetailDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description ?? string.Empty,
                    Price = product.Price,
                    CategoryName = product.Category?.CategoryName ?? string.Empty,
                    SellerName = product.Seller?.FullName ?? string.Empty,
                    Images = product.Images.OrderByDescending(x => x.CreateAt).Select(i => new ImageIndexDto
                    {
                        ImageId = i.ImageId,
                        ImagePath = i.ImagePath ?? string.Empty,
                    }).ToList(),
                    Reviews = product.Reviews.Select(r => new ProductReviewIndexDto
                    {
                        ProductReviewId = r.ReviewId,
                        Rating = r.Rating,
                        Comment = r.Comment ?? string.Empty,
                        CreatedAt = r.CreatedAt,
                        Likes = r.Likes,
                    }).ToList(),
                    Rating = rating
                };
                return ServiceResult<ProductDetailDto>.Success(productDetailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when fetching product by id: {productId}.");
                return ServiceResult<ProductDetailDto>.Error("Lỗi khi thực hiện trả về chi tiết sản phẩm");
            }
        }

        #endregion

        #region Private Methods
        private async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Seller)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
            return products;
        }
        private IEnumerable<ProductIndexDto> MapToProductIndexDtos(IEnumerable<Product> products)
        {
            var productIndexDtos = new List<ProductIndexDto>();
            var escrows = _context.Escrows
                .Where(e => e.Status == EscrowStatus.Done)
                .Include(e => e.Order)
                .ThenInclude(o => o.OrderItems)
                .ToList();
            foreach(var product in products)
            {
                var totalSold = escrows
                    .SelectMany(e => e.Order.OrderItems)
                    .Where(oi => oi.ProductId == product.ProductId)
                    .Sum(oi => oi.Quantity);
                var rating = product.Reviews.Where(x => x.ProductId == product.ProductId)
                    .Average(x => x.Rating);
                var productDto = new ProductIndexDto
                {
                    ProductId = product.ProductId,
                    Image = product.Images.OrderByDescending(x => x.CreateAt).FirstOrDefault()?.ImagePath ?? string.Empty,
                    ProductName = product.ProductName,
                    Category = product.Category?.CategoryName ?? string.Empty,
                    QuantitySelled = totalSold,
                    Price = product.Price,
                    Status = product.Status.ToString(),
                    Rating = rating
                };
                productIndexDtos.Add(productDto);
            }
            return productIndexDtos;
        }
        private IEnumerable<ProductIndexDto> PagingAsync(IEnumerable<ProductIndexDto> products ,int pageNumber, int pageSize)
        {
            return products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        #endregion
    }
}
