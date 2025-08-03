using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ServiceResult<string>>
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IStoreRepository _storeRepository; 
        private readonly IUserRepository _userRepository;

        public UpdateProductHandler(ILogRepository logRepository,
                                    ILogger<UpdateProductHandler> logger,
                                    IProductRepository productRepository,
                                    IProductImageRepository productImageRepository,
                                    IStoreRepository storeRepository,
                                    IUserRepository userRepository)
        {
            _logRepository = logRepository;
            _logger = logger;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProductCommand for StoreId: {StoreId}", request.StoreId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogError("Seller with ID {SellerId} not found", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy người bán");
            }
            if (seller.RoleId != "Seller")
            {
                _logger.LogError("User with ID {SellerId} is not a seller", request.SellerId);
                return ServiceResult<string>.Error("Người dùng không phải là người bán");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
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
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogError("Product with ID {ProductId} not found", request.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm");
            }
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(), 
                Action = "UpdateProduct",
                CreatedAt = DateTime.UtcNow,
                TargetId = product.ProductId,
                TargetType = "Product",
                UserId = request.SellerId,
            };
            try
            {
                product.ProductName = request.ProductName ?? product.ProductName;
                product.SummaryFeature = request.SummaryFeature ?? product.SummaryFeature;
                product.Description = request.Description ?? product.Description;
                product.CategoryId = request.CategoryId ?? product.CategoryId;
                product.OriginalPrice = request.OriginalPrice == 0 ? product.OriginalPrice : request.OriginalPrice;
                if (string.IsNullOrEmpty(request.ProductName) || string.IsNullOrEmpty(request.Description) || string.IsNullOrEmpty(request.SummaryFeature) || string.IsNullOrEmpty(request.CategoryId))
                {
                    product.Status = ProductStatus.Pending;
                }
                await _productRepository.UpdateAsync(product);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thông tin sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", request.ProductId);
                return ServiceResult<string>.Error("Cập nhật sản phẩm thất bại");
            }
        }
    }
}
