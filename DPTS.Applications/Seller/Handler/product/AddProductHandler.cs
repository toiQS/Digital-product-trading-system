using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, ServiceResult<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddProductHandler> _logger;

        public AddProductHandler(IProductRepository productRepository,
                                 IStoreRepository storeRepository,
                                 IProductImageRepository productImageRepository,
                                 ILogRepository logRepository,
                                 IUserRepository userRepository,
                                 ILogger<AddProductHandler> logger)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _productImageRepository = productImageRepository;
            _logRepository = logRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddProductCommand for StoreId: {StoreId}", request.StoreId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogError("Seller with ID {SellerId} not found", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy người bán");
            }
            if(seller.RoleId != "Seller")
            {
                _logger.LogError("User with ID {SellerId} is not a seller", request.SellerId);
                return ServiceResult<string>.Error("Người dùng không phải là người bán");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if(store == null)
            {
                _logger.LogError("Store with ID {StoreId} not found", request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }
            if (store.UserId != request.SellerId)
            {
                _logger.LogError("Store with ID {StoreId} does not belong to SellerId {SellerId}", request.StoreId, request.SellerId);
                return ServiceResult<string>.Error("Cửa hàng không thuộc về người bán");
            }
            if (store.Status != StoreStatus.Active)
            {
                _logger.LogError("Store with ID {StoreId} is not active", request.StoreId);
                return ServiceResult<string>.Error("Cửa hàng không hoạt động");
            }
            var product = new Product()
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = request.ProductName,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now,
                Description = request.Description,
                OriginalPrice = request.OriginalPrice,
                Status = ProductStatus.Pending,
                StoreId = request.StoreId,
                SummaryFeature = request.SumaryFeature
            };
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "AddProduct",
                CreatedAt = DateTime.Now,
                TargetId = product.ProductId,
                TargetType = "Product",
            };
           
            var findingProduct = await _productRepository.GetByNameAndStoreAsync(request.ProductName, request.StoreId);
            if(findingProduct != null)
            {
                _logger.LogError("Product with name {ProductName} already exists in StoreId {StoreId}", request.ProductName, request.StoreId);
                return ServiceResult<string>.Error("Sản phẩm đã tồn tại trong cửa hàng");
            }
            try
            {
                await _productRepository.AddAsync(product);
                foreach (var image in request.Images)
                {
                    var productImage = new ProductImage()
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        ProductId = product.ProductId,
                        ImagePath = image,
                        CreatedAt = DateTime.Now,
                        IsPrimary = true
                    };
                    if (request.Images.First() == image)
                    {
                        productImage.IsPrimary = true;
                    }
                    else
                    {
                        productImage.IsPrimary = false;
                    }

                    await _productImageRepository.AddAsync(productImage);
                }
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Sản phẩm chờ xét duyệt");   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product for StoreId: {StoreId}", request.StoreId);
                return ServiceResult<string>.Error("Lỗi khi thêm sản phẩm");
            }
        }
    }
}
