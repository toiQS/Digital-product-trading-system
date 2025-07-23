using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.product
{
    public class GetSellerProdutsHandle : IRequestHandler<GetSellerProductsItemQuery, ServiceResult<IEnumerable<ProductListItemDto>>>
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

        public GetSellerProdutsHandle(
            IProductRepository productRepository,
            IEscrowRepository escrowRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IProductReviewRepository productReviewRepository,
            ILogger<GetSellerProdutsHandle> logger,
            IStoreRepository storeRepository,
            IProductImageRepository productImageRepository,
            IAdjustmentHandle adjustmentHandle)
        {
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _logger = logger;
            _storeRepository = storeRepository;
            _productImageRepository = productImageRepository;
            _adjustmentHandle = adjustmentHandle ?? throw new ArgumentNullException(nameof(adjustmentHandle));
        }

        public async Task<ServiceResult<IEnumerable<ProductListItemDto>>> Handle(GetSellerProductsItemQuery query, CancellationToken token = default)
        {
            _logger.LogInformation("Handling GetSellerProductsItemQuery for SellerId: {SellerId}", query.SellerId);

            try
            {
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Store not found for SellerId: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Không tìm thấy cửa hàng.");
                }

                var products = await _productRepository.GetByStoreAsync(store.StoreId);

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
                    return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Không thể thống kê sản phẩm đã bán.");
                }

                var result = new List<ProductListItemDto>();

                foreach (var product in products)
                {
                    

                    var productReviews = await _productReviewRepository.GetByProductIdAsync(product.ProductId);
                    var reviewCount = productReviews.Count();
                    var totalRatingOverall = productReviews.Sum(x => x.RatingOverall);
                    var RatingOverallAvg = reviewCount > 0 ? (double)totalRatingOverall / reviewCount : 0;

                    var quantitySold = soldQuantities.FirstOrDefault(x => x.ProductId == product.ProductId)?.TotalQuantity ?? 0;

                    var productImage = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                    if (productImage == null)
                    {
                        _logger.LogError("ProductId {ProductId} has no primary image", product.ProductId);
                        return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Không lấy được ảnh của sản phẩm");
                    }

                    var finalPrice = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
                    if (finalPrice.Status == StatusResult.Errored)
                    {
                        _logger.LogError("Price adjustment failed for ProductId {ProductId}: {Message}", product.ProductId, finalPrice.MessageResult);
                        return ServiceResult<IEnumerable<ProductListItemDto>>.Error(finalPrice.MessageResult);
                    }

                    result.Add(new ProductListItemDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Category = product.Category?.CategoryName ?? "Không xác định",
                        Price = finalPrice.Data.FinalAmount,
                        Image = productImage.ImagePath,
                        QuantitySelled = quantitySold,
                        Status = EnumHandle.HandleProductStatus(product.Status),
                        RatingOverall = RatingOverallAvg
                    });
                }

                var paginatedResult = result
                    .OrderByDescending(x => x.QuantitySelled)
                    .Skip((query.PageCount - 1) * query.PageSize)
                    .Take(query.PageSize);

                _logger.LogInformation("Successfully returned {Count} products for SellerId: {SellerId}", result.Count, query.SellerId);
                return ServiceResult<IEnumerable<ProductListItemDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetSellerProductsHandler.");
                return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Đã xảy ra lỗi khi lấy sản phẩm.");
            }
        }

        private class ProductIndexQuantity
        {
            public string ProductId { get; set; } = string.Empty;
            public int TotalQuantity { get; set; }
        }
    }
}
