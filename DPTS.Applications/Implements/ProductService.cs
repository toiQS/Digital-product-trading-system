using DPTS.Applications.Dtos.products;
using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DPTS.Applications.Implements
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("");
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                if (!products.Any()) return ServiceResult<IEnumerable<ProductIndexModel>>.Success(null!);
                List<ProductIndexModel> productIndexModels = new List<ProductIndexModel>();
                foreach (var product in products)
                {
                    var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), product.ProductId);
                    var category =await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId);
                    if (category == null) return ServiceResult<IEnumerable<ProductIndexModel>>.Error("");
                    var escrows = await _unitOfWork.Repository<Escrow>().GetAllAsync();
                    var quantitySelled = 0;
                    foreach (var escrow in escrows.Where(x => x.Status == StatusEntity.Done))
                    {
                        var orderItems = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), escrow.OrderId);
                        quantitySelled = orderItems.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);

                    }
                    productIndexModels.Add(new ProductIndexModel()
                    {
                        Image = images.First().ImagePath,
                        Name = product.Title,
                        Category = category.Name,
                        Price = product.Price,
                        QuantitySelled = quantitySelled,
                    });
                }
                return ServiceResult<IEnumerable<ProductIndexModel>>.Success(productIndexModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<ProductIndexModel>>.Error("");
            }
        }
        public async Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsBySellerId(string sellerId)
        {
            try
            {
                _logger.LogInformation("");
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                if (!products.Any()) return ServiceResult<IEnumerable<ProductIndexModel>>.Success(null!);
                List<ProductIndexModel> productIndexModels = new List<ProductIndexModel>();
                foreach (var product in products)
                {
                    var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), product.ProductId);
                    var category = await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId);
                    if (category == null) return ServiceResult<IEnumerable<ProductIndexModel>>.Error("");
                    var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync(nameof(Escrow.SellerId),sellerId);
                    var quantitySelled = 0;
                    foreach (var escrow in escrows.Where(x => x.Status == StatusEntity.Done))
                    {
                        var orderItems = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), escrow.OrderId);
                        quantitySelled = orderItems.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);

                    }
                    
                    productIndexModels.Add(new ProductIndexModel()
                    {
                        Image = images.First().ImagePath,
                        Name = product.Title,
                        Category = category.Name,
                        Price = product.Price,
                        QuantitySelled = quantitySelled,
                    });
                }
                return ServiceResult<IEnumerable<ProductIndexModel>>.Success(productIndexModels);
            }
            
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<ProductIndexModel>>.Error("");
            }
        }
        public async Task<ServiceResult<IEnumerable<ProductIndexModel>>> ProductBestSellerAsync()
        {
            try
            {
                _logger.LogInformation("");
                var escrows = await _unitOfWork.Repository<Escrow>().GetAllAsync();
                if (!escrows.Any()) return ServiceResult<IEnumerable<ProductIndexModel>>.Success(null!);
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                if (!products.Any() && escrows.Any()) return ServiceResult<IEnumerable<ProductIndexModel>>.Error("");


                var result = new List<ProductIndexModel>();

                foreach (var product in products)
                {
                    int totalSold = 0;
                    var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), product.ProductId);

                    foreach (var escrow in escrows.Where(x => x.Status == StatusEntity.Done))
                    {
                        var items = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), escrow.OrderId);
                        totalSold += items.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);
                    }
                    var productRating = await _unitOfWork.Repository<ProductReview>().GetManyAsync(nameof(ProductReview.ProductId), product.ProductId);
                    
                    var category = await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId!);
                    result.Add(new ProductIndexModel
                    {
                        Category = category?.Name ?? "Unknown",
                        Name = product.Title,
                        Image = images.FirstOrDefault()?.ImagePath ?? string.Empty,
                        Price = product.Price,
                        QuantitySelled = totalSold,
                        Rating = (float) productRating.Sum(x => x.Rating)!/totalSold,
                    });
                }
                result.OrderByDescending(x => x.QuantitySelled);

                return ServiceResult<IEnumerable<ProductIndexModel>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thống kê sản phẩm bán chạy.");
                return ServiceResult<IEnumerable<ProductIndexModel>>.Error("Lỗi khi lấy danh sách sản phẩm bán chạy.");
            }
        }
    }
}
