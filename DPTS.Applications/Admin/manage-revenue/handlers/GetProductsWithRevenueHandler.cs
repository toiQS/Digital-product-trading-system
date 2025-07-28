using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Admin.manage_revenue.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_revenue.handlers
{
    public class GetProductsWithRevenueHandler : IRequestHandler<GetProductsWithRevenueQuery,ServiceResult<ProductWithRevenueDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogger<GetProductsWithRevenueHandler> _logger;

        public GetProductsWithRevenueHandler(IUserRepository userRepository,
                                             IProductRepository productRepository,
                                             IEscrowRepository ecrowRepository,
                                             IOrderItemRepository orderItemRepository,
                                             ILogger<GetProductsWithRevenueHandler> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _ecrowRepository = ecrowRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductWithRevenueDto>> Handle(GetProductsWithRevenueQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get products and sort with revenue");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<ProductWithRevenueDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<ProductWithRevenueDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var privateResult = new List<PrivateResul>();
            var products = await _productRepository.SearchAsync();
            privateResult = products.Select(x => new PrivateResul()
            {
                Name = x.ProductName,
                Id = x.ProductId,
                Revenue = 0m,
                Sold = 0,
            }).ToList();
            var escrows = (await _ecrowRepository.GetAllAsync()).Where(x => x.Status == Domains.EscrowStatus.Done);
            foreach (var escrow in escrows)
            {
                var items = await _orderItemRepository.GetByOrderIdAsync(escrow.OrderId);
                items.ForEach(i =>
                {
                    var item = privateResult.FirstOrDefault(x => x.Id == i.ProductId);
                    item.Revenue += i.TotalPrice;
                    item.Sold += i.Quantity;
                });
            }
            var result = new ProductWithRevenueDto()
            {
                Indexs = privateResult.Select(x => new ProductWithRevenueIndexDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Revenue = ConvertUnit(x.Revenue),
                    Sold= x.Sold,
                }).ToList(),
            };
            return ServiceResult<ProductWithRevenueDto>.Success(result);
        }
        private class PrivateResul
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Sold { get; set; }
            public decimal Revenue { get; set; }
        }
        private string ConvertUnit(decimal value)
        {
            if (value < 1000000)
                return value + " vnđ";
            if (value >= 1000000 && value < 9999999)
                return value / 1000000 + " triệu vnđ";

            if (value >= 1000000000 && value < 9999999999)
                return value / 1000000 + " tỉ vnđ";
            else return "Vượt hạn mức";
        }
    }
}
