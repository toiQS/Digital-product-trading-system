using DPTS.Applications.Sellers.revenues.Dtos;
using DPTS.Applications.Sellers.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, ServiceResult<IEnumerable<TopSellingProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetTopSellingProductsHandler> _logger;
    private readonly IStoreRepository _storeRepository;
    private readonly IEscrowRepository _ecrowRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductImageRepository _productImageRepository;

    public GetTopSellingProductsHandler(
        IProductRepository productRepository,
        ILogger<GetTopSellingProductsHandler> logger,
        IStoreRepository storeRepository,
        IEscrowRepository ecrowRepository,
        IUserProfileRepository userProfileRepository,
        IOrderItemRepository orderItemRepository,
        IProductImageRepository productImageRepository)
    {
        _productRepository = productRepository;
        _logger = logger;
        _storeRepository = storeRepository;
        _ecrowRepository = ecrowRepository;
        _userProfileRepository = userProfileRepository;
        _orderItemRepository = orderItemRepository;
        _productImageRepository = productImageRepository;
    }

    public async Task<ServiceResult<IEnumerable<TopSellingProductDto>>> Handle(GetTopSellingProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating top selling products for seller {SellerId}", request.SellerId);

        // Validate
        var profile = await _userProfileRepository.GetByUserIdAsync(request.SellerId);
        if (profile == null)
        {
            _logger.LogWarning("Seller profile not found: {SellerId}", request.SellerId);
            return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Không tìm thấy người bán.");
        }

        var store = await _storeRepository.GetByUserIdAsync(profile.UserId);
        if (store == null)
        {
            _logger.LogWarning("Store not found for SellerId = {SellerId}", request.SellerId);
            return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Không tìm thấy cửa hàng.");
        }

        // Get all escrows & orderIds
        var escrows = await _ecrowRepository.GetAllAsync(storeId: store.StoreId);
        var orderIds = escrows.Select(e => e.OrderId).Distinct().ToList();

        // Get all order items in 1 call
        var allOrderItems = new List<OrderItem>();
        foreach (var orderId in orderIds)
        {
            var items = await _orderItemRepository.GetByOrderIdAsync(orderId);
            allOrderItems.AddRange(items);
        }

        // Group by ProductId
        var productSales = allOrderItems
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.PriceForeachProduct)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .ToList();

        // Fetch all related products + images
        var productIds = productSales.Select(x => x.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByListIdsAsync(productIds);
        var imagesDict = new Dictionary<string, ProductImage>();

        foreach (var pid in productIds)
        {
            var img = await _productImageRepository.GetPrimaryAsync(pid);
            if (img != null)
                imagesDict[pid] = img;
        }

        // Build result
        var result = new List<TopSellingProductDto>();

        foreach (var item in productSales)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Missing product for ProductId: {ProductId}", item.ProductId);
                continue;
            }

            imagesDict.TryGetValue(item.ProductId, out var image);

            result.Add(new TopSellingProductDto
            {
                ProductId = item.ProductId,
                ProductName = product.ProductName,
                OrderCount = item.TotalQuantity,
                Revenue = item.TotalRevenue,
                ImageUrl = image?.ImagePath ?? ""
            });
        }

        return ServiceResult<IEnumerable<TopSellingProductDto>>.Success(result);
    }
}
