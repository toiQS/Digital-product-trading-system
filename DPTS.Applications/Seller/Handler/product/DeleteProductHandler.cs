using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<DeleteProductHandler> _logger;
        private readonly ILogRepository _logRepository;

        public DeleteProductHandler(IUserRepository userRepository,
                                    IStoreRepository storeRepository,
                                    IProductRepository productRepository,
                                    ILogger<DeleteProductHandler> logger,
                                    ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProductCommand for ProductId: {ProductId}, StoreId: {StoreId}, SellerId: {SellerId}", 
                request.ProductId, request.StoreId, request.SellerId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if(seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found.", request.SellerId);
                return ServiceResult<string>.Error($"Không tìm thấy người bán.");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if(store == null)
            {
                _logger.LogWarning("Store with ID {StoreId} not found.", request.StoreId);
                return ServiceResult<string>.Error($"Không tìm thấy cửa hàng.");
            }
            if(store.UserId != request.SellerId)
            {
                _logger.LogWarning("Store with ID {StoreId} does not belong to SellerId {SellerId}.", request.StoreId, request.SellerId);
                return ServiceResult<string>.Error($"Cửa hàng không thuộc về người bán.");
            }
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogError("Product with ID {ProductId} not found.", request.ProductId);
                return ServiceResult<string>.Error($"Không tìm thấy sản phẩm.");
            }
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "DeleteProduct",
                CreatedAt = DateTime.UtcNow,
                TargetId = product.ProductId,
                TargetType = "Product",
                UserId = request.SellerId,
            };
            try
            {
                product.Status = Domains.ProductStatus.Deleted;
                await _productRepository.UpdateAsync(product);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Sản phẩm đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}.", request.ProductId);
                return ServiceResult<string>.Error($"Lỗi khi xóa sản phẩm: {ex.Message}");
            }
        }
    }
}
