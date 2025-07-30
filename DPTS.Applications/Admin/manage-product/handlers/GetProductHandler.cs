using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Admin.manage_product.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_product.handlers
{
    public class GetProductHandler : IRequestHandler<GetProductsQuery, ServiceResult<ProductDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetProductHandler> _logger;
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly IProductImageRepository _productImageRepository;

        public GetProductHandler(IUserRepository userRepository,
                                 IProductRepository productRepository,
                                 IStoreRepository storeRepository,
                                 ICategoryRepository categoryRepository,
                                 ILogger<GetProductHandler> logger,
                                 IAdjustmentHandle adjustmentHandle,
                                 IProductImageRepository productImageRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _adjustmentHandle = adjustmentHandle;
            _productImageRepository = productImageRepository;
        }

        public async Task<ServiceResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get products with condition");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<ProductDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<ProductDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var products = await _productRepository.SearchAsync();
           
            if (request.Condition.ProductStatus != Domains.ProductStatus.Unknown)
            {
                products = products.Where(x => x.Status == request.Condition.ProductStatus);
            }
            var result = new ProductDto();
            foreach (var p in products)
            {
                var image = await _productImageRepository.GetPrimaryAsync(p.ProductId);
                if (image == null)
                {
                    _logger.LogError("Error when get information of image");
                }

                var store = await _storeRepository.GetByIdAsync(p.StoreId);
                if (store == null)
                {
                    _logger.LogError("Error when get information of store");
                }

                var category = await _categoryRepository.GetByIdAsync(p.CategoryId);
                if (category == null)
                {
                    _logger.LogError("Error when get information of category");
                }

                var finalPrice = await _adjustmentHandle.HandleDiscountAndPriceForProduct(p);

                var index = new ProductIndexDto()
                {
                    CategoryId = p.CategoryId,
                    CategoryName = category?.CategoryName ?? "Error",
                    ProductName = p.ProductName,
                    StoreName = store?.StoreName ?? "Error",
                    Price = finalPrice.Data.FinalAmount,
                    ProductId = p.ProductId,
                    ProductImage = image?.ImagePath ?? "Error",
                    Status = EnumHandle.HandleProductStatus(p.Status),
                    StoreId = store?.StoreId ?? "Error"
                };

                result.ProductIndexDtos.Add(index);
            }

            if (request.Condition.Text != null)
            {
                result.ProductIndexDtos = result.ProductIndexDtos.Where(x =>
                {
                    return x.ProductName.Contains(request.Condition.Text) || x.StoreName.Contains(request.Condition.Text) || x.ProductId.Contains(request.Condition.Text);
                }).ToList();
            }
            result.ProductIndexDtos = result.ProductIndexDtos.OrderByDescending(x => x.Price).ToList();
            result.ProductCount = result.ProductIndexDtos.Count; 
            if (request.PageNumber > 0 && request.PageSize > 0)
            {
                result.ProductIndexDtos = result.ProductIndexDtos.Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize).ToList();
            }
            return ServiceResult<ProductDto>.Success(result);
        }
    }
}
