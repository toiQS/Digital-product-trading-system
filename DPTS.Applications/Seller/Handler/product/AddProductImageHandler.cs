using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class AddProductImageHandler : IRequestHandler<AddProductImageCommand, ServiceResult<string>>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddProductImageHandler> _logger;   
        private readonly ILogRepository _logRepository;

        public AddProductImageHandler(IProductImageRepository productImageRepository,
                                      IProductRepository productRepository,
                                      IStoreRepository storeRepository,
                                      IUserRepository userRepository,
                                      ILogger<AddProductImageHandler> logger,
                                      ILogRepository logRepository)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddProductImageCommand for SellerId: {SellerId}, StoreId: {StoreId}, ProductId: {ProductId}", 
                request.SellerId, request.StoreId, request.ProductId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if(seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found.", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy người bán");

            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogWarning("Store with ID {StoreId} not found.", request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }
            if(store.UserId != request.SellerId)
            {
                _logger.LogWarning("Store with ID {StoreId} does not belong to SellerId: {SellerId}.", request.StoreId, request.SellerId);
                return ServiceResult<string>.Error("Cửa hàng không thuộc về người bán này");
            }
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", request.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm");
            }
            var checkProductImage = (await _productImageRepository.GetByProductIdAsync(request.ProductId)).Where(x => x.ImagePath == request.ImagePath);
            if (checkProductImage.Any())
            {
                _logger.LogWarning("Image {ImagePath} already exists for ProductId {ProductId}", request.ImagePath, request.ProductId);
                return ServiceResult<string>.Error("Ảnh đã tồn tại trong sản phẩm");
            }


            var productImage = new ProductImage()
            {
                CreatedAt = DateTime.UtcNow,
                ImageId = Guid.NewGuid().ToString(),
                ImagePath = request.ImagePath,
                IsPrimary = false,
                ProductId = product.ProductId
            };
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                Action ="AddProductImage",
                CreatedAt= DateTime.UtcNow,
                TargetId = store.StoreId,
                TargetType = "Store"
            };
            try
            {
                await _productImageRepository.AddAsync(productImage);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Thêm ảnh thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error when adding product image");
                return ServiceResult<string>.Error("Error when adding product image");
            }
        }
    }
}
