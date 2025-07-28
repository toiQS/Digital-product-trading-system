using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Admin.manage_revenue.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace DPTS.Applications.Admin.manage_revenue.handlers
{
    public class GetRevenueAnalysisHandler : IRequestHandler<GetRevenueAnalysisQuery, ServiceResult<RevenueAnalysisDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetDayOfWeekSalesChartHandler> _logger;

        public GetRevenueAnalysisHandler(IUserRepository userRepository,
                                         IEscrowRepository ecrowRepository,
                                         ICategoryRepository categoryRepository,
                                         IOrderItemRepository orderItemRepository,
                                         IProductRepository productRepository,
                                         ILogger<GetDayOfWeekSalesChartHandler> logger)
        {
            _userRepository = userRepository;
            _ecrowRepository = ecrowRepository;
            _categoryRepository = categoryRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<RevenueAnalysisDto>> Handle(GetRevenueAnalysisQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get revenue analysis");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<RevenueAnalysisDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<RevenueAnalysisDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var categories = await _categoryRepository.GetsAsync();
            var escrows = (await _ecrowRepository.GetAllAsync()).Where(x => x.Status == Domains.EscrowStatus.Done).ToList();
            var categoryResult = categories.Select(x => new CategoryResult()
            {
                Id = x.CategoryId,
                Name = x.CategoryName,
                Amount = 0
            }).ToList();

            foreach (var escrow in escrows)
            {
                var items = await _orderItemRepository.GetByOrderIdAsync(escrow.OrderId);
                foreach (var i in items)
                {
                    var product = await _productRepository  .GetByIdAsync(i.ProductId);
                    if (product == null)
                    {
                        _logger.LogError("Not found product with Id:{id}", product.ProductId);
                    }
                    var getCategory = categoryResult.FirstOrDefault(x => x.Id == product.CategoryId);
                    getCategory.Amount += i.TotalPrice;

                }
            }
            var result = new RevenueAnalysisDto()
            {
                Indexs = categoryResult.Select(x => new RevenueAnalysisIndexDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Value = ConvertUnit(x.Amount)
                }).ToList()
            };
            return ServiceResult<RevenueAnalysisDto>.Success(result);
        }
        public class CategoryResult
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
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
