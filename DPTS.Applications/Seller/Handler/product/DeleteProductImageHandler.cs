using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand, ServiceResult<string>>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeleteProductImageHandler> _logger;
        private readonly ILogRepository _logRepository;

        public DeleteProductImageHandler(IProductImageRepository productImageRepository, IProductRepository productRepository, IStoreRepository storeRepository, IUserRepository userRepository, ILogger<DeleteProductImageHandler> logger, ILogRepository logRepository)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProductImageCommand for SellerId: {SellerId}, StoreId: {StoreId}, ProductId: {ProductId}, ImageId: {ImageId}", 
                request.SellerId, request.StoreId, request.ProductId, request.ImageId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogError("Seller with ID {SellerId} not found.", request.SellerId);  
                return ServiceResult<string>.Error("Không tìm thấy người bán");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogError("Store with ID {StoreId} not found.", request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }
            if (store.UserId != request.SellerId)
            {
                _logger.LogError("Store with ID {StoreId} does not belong to SellerId: {SellerId}.", request.StoreId, request.SellerId);
                return ServiceResult<string>.Error("Cửa hàng không thuộc về người bán này");
            }
            var product = await _productRepository.GetByIdAsync(request.ProductId); 
            if (product == null)
            {
                _logger.LogError("Product with ID {ProductId} not found.", request.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm");
            }
            if(product.StoreId != request.StoreId)
            {
                _logger.LogError("Product with ID {ProductId} does not belong to StoreId: {StoreId}.", request.ProductId, request.StoreId);
                return ServiceResult<string>.Error("Sản phẩm không thuộc về cửa hàng này");
            }
            var productImage = await _productImageRepository.GetByIdAsync(request.ImageId); 
            if(productImage == null)
            {
                _logger.LogError("Product image with ID {ImageId} not found.", request.ImageId);
                return ServiceResult<string>.Error("Không tìm thấy hình ảnh sản phẩm");
            }
            try
            {
                var log = new Log()
                {
                    LogId = Guid.NewGuid().ToString(),
                    Action = "DeleteProductImage",
                    CreatedAt = DateTime.Now,
                    TargetId = productImage.ImageId,
                    TargetType = "ProductImage"
                };
                await _productImageRepository.DeleteAsync(productImage.ImageId);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Xóa ảnh sản phẩm thành công");    
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting product image with ID {ImageId}", request.ImageId);
                return ServiceResult<string>.Error("Xóa ảnh sản phẩm thất bại");
            }
        }
    }
}
