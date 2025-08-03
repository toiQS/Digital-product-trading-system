using DPTS.Applications.Admin.manage_product.Queries;
using DPTS.Applications.Auth.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql.Replication.PgOutput;

namespace DPTS.Applications.Admin.manage_product.handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(IUserRepository userRepository, IProductRepository productRepository, ILogRepository logRepository, ILogger<UpdateProductHandler> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handing update product");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<string>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var product= await _productRepository.GetByIdAsync(request.ConditionUpdateProduct.ProductId);
            if (product == null)
            {
                _logger.LogError("Not found product with Id:{id}",request.ConditionUpdateProduct.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm");

            }
            var log = new Log()
            {
                LogId  = Guid.NewGuid().ToString(),
                Action = "Cập nhật sản phẩm",
                CreatedAt = DateTime.UtcNow,
                Description = $"{request.ConditionUpdateProduct.ProductId} được cập nhật trạng thái bởi {request.UserId}",
                TargetId = request.ConditionUpdateProduct.ProductId,
                TargetType = "Product",
                UserId = request.UserId,
            };
            try
            {
                await _productRepository.UpdateAsync(product);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error when update product");
                return ServiceResult<string>.Error("Lỗi khi cập nhật sản phẩm");
            }
        }
    }
}
