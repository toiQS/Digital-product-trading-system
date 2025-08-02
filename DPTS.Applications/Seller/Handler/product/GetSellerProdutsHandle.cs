using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql.Replication;

namespace DPTS.Applications.Seller.Handler.product
{
    public class GetSellerProdutsHandle : IRequestHandler<GetSellerProductsItemQuery, ServiceResult<ProductListDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ILogger<GetSellerProdutsHandle> _logger;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly ICategoryRepository _categoryRepository;

        public GetSellerProdutsHandle(IProductRepository productRepository,
                                      IEscrowRepository escrowRepository,
                                      IOrderItemRepository orderItemRepository,
                                      IOrderRepository orderRepository,
                                      IProductReviewRepository productReviewRepository,
                                      ILogger<GetSellerProdutsHandle> logger,
                                      IStoreRepository storeRepository,
                                      IProductImageRepository productImageRepository,
                                      IAdjustmentHandle adjustmentHandle,
                                      ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _logger = logger;
            _storeRepository = storeRepository;
            _productImageRepository = productImageRepository;
            _adjustmentHandle = adjustmentHandle;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<ProductListDto>> Handle(GetSellerProductsItemQuery query, CancellationToken token = default)
        {
            _logger.LogInformation("Handling GetSellerProductsItemQuery for SellerId: {SellerId}", query.SellerId);

            var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for SellerId: {SellerId}", query.SellerId);
                return ServiceResult<ProductListDto>.Error("Không tìm thấy cửa hàng.");
            }

            var products = await _productRepository.GetByStoreAsync(store.StoreId);
            if (query.Condition.CategoryId != null!)
                products = products.Where(x => x.CategoryId == query.Condition.CategoryId).ToList();
            if (query.Condition.Status != Domains.ProductStatus.Unknown)
                products = products.Where(x => x.Status == query.Condition.Status);
            if (!string.IsNullOrEmpty(query.Condition.Text))
                products = products.Where(x => x.ProductName.ToLower().Contains(query.Condition.Text.ToLower())).ToList();
            var escrows = await _escrowRepository.GetAllAsync(storeId: store.StoreId, status: Domains.EscrowStatus.Done);
            var orderItems = await _orderItemRepository.GetAllAsync();

            List<ProductIndexQuantity> soldQuantities;
            try
            {
                soldQuantities = (
                    from escrow in escrows
                    where escrow.Order != null && escrow.Order.IsPaid
                    join item in orderItems on escrow.Order.OrderId equals item.OrderId
                    group item by item.ProductId into g
                    select new ProductIndexQuantity
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to group sold quantity per product.");
                return ServiceResult<ProductListDto>.Error("Không thể thống kê sản phẩm đã bán.");
            }

            var result = new ProductListDto();

            foreach (var product in products)
            {
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                if (category == null)
                {
                    _logger.LogError("Error when try get information of category");
                }

                var productReviews = await _productReviewRepository.GetByProductIdAsync(product.ProductId);
                var reviewCount = productReviews.Count();
                var totalRatingOverall = productReviews.Sum(x => x.RatingOverall);
                var RatingOverallAvg = reviewCount > 0 ? (double)totalRatingOverall / reviewCount : 0;

                var quantitySold = soldQuantities.FirstOrDefault(x => x.ProductId == product.ProductId)?.TotalQuantity ?? 0;

                var productImage = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                if (productImage == null)
                {
                    _logger.LogError("ProductId {ProductId} has no primary image", product.ProductId);
                    return ServiceResult<ProductListDto>.Error("Không lấy được ảnh của sản phẩm");
                }

                var finalPrice = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
                if (finalPrice.Status == StatusResult.Errored)
                {
                    _logger.LogError("Price adjustment failed for ProductId {ProductId}: {Message}", product.ProductId, finalPrice.MessageResult);
                    return ServiceResult<ProductListDto>.Error(finalPrice.MessageResult);
                }

                result.Items.Add(new ProductListItemDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Category = category.CategoryName ?? "Không xác định",
                    Price = finalPrice.Data.FinalAmount,
                    Image = productImage.ImagePath,
                    QuantitySelled = quantitySold,
                    Status = EnumHandle.HandleProductStatus(product.Status),
                    RatingOverall = RatingOverallAvg
                });

            }

            result.Total = result.Items.Count;
            if (query.PageCount > 0 && query.PageSize > 0)
            {
                result.Items = result.Items
                .OrderByDescending(x => x.QuantitySelled)
                .Skip((query.PageCount - 1) * query.PageSize)
                .Take(query.PageSize).ToList();
            }
            result.Count = result.Items.Count;   
            _logger.LogInformation("Successfully returned {Count} products for SellerId: {SellerId}", result.Count, query.SellerId);
            return ServiceResult<ProductListDto>.Success(result);
        }

        private class ProductIndexQuantity
        {
            public string ProductId { get; set; } = string.Empty;
            public int TotalQuantity { get; set; }
        }
    }
}
