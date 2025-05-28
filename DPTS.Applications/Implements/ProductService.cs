using DPTS.Applications.Dtos.images;
using DPTS.Applications.Dtos.products;
using DPTS.Applications.Dtos.reviews;
using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Reflection.Metadata.Ecma335;
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
                    var category = await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId);
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
                    var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync(nameof(Escrow.SellerId), sellerId);
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
                        Rating = (float)productRating.Sum(x => x.Rating)! / totalSold,
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
        public async Task<ServiceResult<ProductDetailModel>> DetailProductAsync(string productId)
        {
            try
            {
                _logger.LogInformation("");
                var product = await _unitOfWork.Repository<Product>().GetOneAsync(nameof(Product.ProductId), productId);
                if (product == null) return ServiceResult<ProductDetailModel>.Error("Sản phẩm không tồn tại.");
                var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), productId);
                var category =await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId);
                if (category == null) return ServiceResult<ProductDetailModel>.Error("Danh mục sản phẩm không tồn tại.");
                var reviews = await _unitOfWork.Repository<ProductReview>().GetManyAsync(nameof(ProductReview.ProductId), productId);
                var averageRating = reviews.Any() ? reviews.Average(x => x.Rating) : 0;
                var detailModel = new ProductDetailModel
                {
                    ProductId = product.ProductId,
                    Title = product.Title,
                    Description = product.Description!,
                    Price = product.Price,
                    CategoryName = category.Name,
                    Images = images.Select(img => new IndexImageModel
                    {
                        ImageId = img.ImageId,
                        ImagePath = img.ImagePath
                    }).ToList(),
                    Rating = (float)averageRating,
                    Status = new EnumHandle().ParseEnumToString(product.Status),

                };
                foreach (var item in reviews)
                {
                    var buyer = await _unitOfWork.Repository<User>().GetOneAsync(nameof(User.UserId), item.UserId);

                    var index = new IndexProductReviewModel()
                    {
                        Content = item.Comment!,
                        Likes = item.Likes,
                        Rating = item.Rating ?? 0,
                        ProductReviewId = item.ReviewId,
                        BuyerName = buyer?.FullName ?? "Unknown",
                    };
                    detailModel.Reviews.Add(index);
                }
                return ServiceResult<ProductDetailModel>.Success(detailModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết sản phẩm.");
                return ServiceResult<ProductDetailModel>.Error("Lỗi khi lấy chi tiết sản phẩm.");
            }
        }
        public async Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsByCategoryIdAndRating(string categoryId, int rating)
        {

            try
            {
                _logger.LogInformation("Fetching products by category ID and rating.");
                var productIndeModels = new List<ProductIndexModel>();
                var products = new List<Product>();
                if (string.IsNullOrEmpty(categoryId))
                {
                    var getProducts = await _unitOfWork.Repository<Product>().GetAllAsync();
                    products = getProducts.ToList();
                }
                else
                {
                    var getProducts = await _unitOfWork.Repository<Product>().GetManyAsync(nameof(Product.CategoryId), categoryId);
                    products = getProducts.ToList();
                }

                foreach(var product in products)
                {
                    var reviews = await _unitOfWork.Repository<ProductReview>().GetManyAsync(nameof(ProductReview.ProductId), product.ProductId);
                    if(reviews.Any() && reviews.Average(x => x.Rating) >= rating)
                    { 
                     var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), product.ProductId);
                        var category = await _unitOfWork.Repository<Category>().GetOneAsync(nameof(Category.CategoryId), product.CategoryId);
                        if (category == null) return ServiceResult<IEnumerable<ProductIndexModel>>.Error("Danh mục sản phẩm không tồn tại.");
                        var escrows = await _unitOfWork.Repository<Escrow>().GetAllAsync();
                        var quantitySelled = 0;
                        foreach (var escrow in escrows.Where(x => x.Status == StatusEntity.Done))
                        {
                            var orderItems = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), escrow.OrderId);
                            quantitySelled = orderItems.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);
                        }
                        productIndeModels.Add(new ProductIndexModel()
                        {
                            Image = images.First().ImagePath,
                            Name = product.Title,
                            Category = category.Name,
                            Price = product.Price,
                            QuantitySelled = quantitySelled,
                            Rating = (float)reviews.Average(x => x.Rating)!
                        });
                    }
                }
               return ServiceResult<IEnumerable<ProductIndexModel>>.Success(productIndeModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm theo danh mục và đánh giá.");
                return ServiceResult<IEnumerable<ProductIndexModel>>.Error("Lỗi khi lấy sản phẩm theo danh mục và đánh giá.");
            }

        }
        public async Task<ServiceResult<IEnumerable<ProductIndexModel>>> CanBeLiked(string categoryId)
        {
            var productIndexModels = await GetProductsByCategoryIdAndRating(categoryId, 4);
            var take5 = productIndexModels.Data?.Take(5).ToList() ?? new List<ProductIndexModel>();
            if (take5.Count == 0)
            {
                return ServiceResult<IEnumerable<ProductIndexModel>>.Error("Không có sản phẩm nào phù hợp.");
            }
            return ServiceResult<IEnumerable<ProductIndexModel>>.Success(take5);
        }
    }
}
