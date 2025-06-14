using DPTS.Applications.Seller.products.Dtos;
using DPTS.Applications.Seller.products.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.products.Handles
{
    public class SearchSellerProductsHandle : IRequestHandler<SearchSellerProductsQuery, ServiceResult<IEnumerable<ProductListItemDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ILogger<SearchSellerProductsHandle> _logger;

        public SearchSellerProductsHandle(
            IProductRepository productRepository,
            IEscrowRepository escrowRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IProductReviewRepository productReviewRepository,
            ILogger<SearchSellerProductsHandle> logger)
        {
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ProductListItemDto>>> Handle(SearchSellerProductsQuery query, CancellationToken token = default)
        {
            try
            {
                // Lấy danh sách sản phẩm của người bán theo tiêu chí tìm kiếm
                var products = await _productRepository.GetsAsync(
                    sellerId: query.SellerId,
                    text: query.Text,
                    status: query.Status);

                // Lấy danh sách escrow đã hoàn thành (done) của người bán
                var escrows = await _escrowRepository.GetsAsync(
                    sellerId: query.SellerId,
                    status: Domains.EscrowStatus.Done);

                // Lấy toàn bộ các item trong đơn hàng
                var orderItems = await _orderItemRepository.GetsAsync();

                // Gộp thông tin escrow và orderItem để tính tổng số lượng đã bán theo từng sản phẩm
                List<ProductIndexQuantity> soldQuantities;
                try
                {
                    soldQuantities = (
                        from escrow in escrows
                        where escrow.Order.IsPaied
                        let order = escrow.Order
                        join item in orderItems on order.OrderId equals item.OrderId
                        group item by item.ProductId into grouped
                        select new ProductIndexQuantity
                        {
                            ProductId = grouped.Key,
                            TotalQuantity = grouped.Sum(x => x.Quantity)
                        }
                    ).ToList();
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu quá trình tính toán số lượng bán ra thất bại
                    _logger.LogError(ex, "Failed to compute quantity sold per product.");
                    return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Lỗi khi tính số lượng sản phẩm đã bán.");
                }

                var result = new List<ProductListItemDto>();

                // Duyệt từng sản phẩm và dựng DTO kết quả
                foreach (var product in products)
                {
                    // Lấy danh sách đánh giá sản phẩm
                    var productReviews = await _productReviewRepository.GetsAsync(productId: product.ProductId);
                    var reviewCount = productReviews.Count();
                    var totalRating = productReviews.Sum(x => x.Rating);

                    // Tìm ảnh đại diện (primary image)
                    var primaryImage = product.Images.FirstOrDefault(x => x.IsPrimary)?.ImagePath ?? "N/A";

                    // Tính tổng số lượng đã bán
                    var quantitySold = soldQuantities
                        .FirstOrDefault(x => x.ProductId == product.ProductId)?.TotalQuantity ?? 0;

                    // Tạo DTO
                    result.Add(new ProductListItemDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Category = product.Category.CategoryName,
                        Price = product.Price,
                        Image = primaryImage,
                        QuantitySelled = quantitySold,
                        Status = EnumHandle.HandleProductStatus(product.Status),
                        Rating = reviewCount > 0 ? (double)totalRating / reviewCount : 0
                    });
                }

                // Trả về danh sách đã phân trang theo số lượng đã bán
                var pagedResult = result
                    .OrderByDescending(x => x.QuantitySelled)
                    .Skip((query.PageCount - 1) * query.PageSize)
                    .Take(query.PageSize);

                return ServiceResult<IEnumerable<ProductListItemDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi không xác định
                _logger.LogError(ex, "Unhandled exception in SearchSellerProductsHandle.Handle.");
                return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Đã xảy ra lỗi khi lấy danh sách sản phẩm.");
            }
        }

        /// <summary>
        /// Class nội bộ để gom nhóm số lượng sản phẩm đã bán
        /// </summary>
        private class ProductIndexQuantity
        {
            public string ProductId { get; set; } = string.Empty;
            public int TotalQuantity { get; set; }
        }
    }
}
