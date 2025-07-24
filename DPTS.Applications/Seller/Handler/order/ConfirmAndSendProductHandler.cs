using DPTS.Applications.Seller.Query.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DPTS.Applications.Seller.Handler.order
{
    public class ConfirmAndSendProductHandler : IRequestHandler<ConfirmAndSendProductCommand, ServiceResult<string>>
    {
        private readonly IEscrowProcessRepository _escrowProcessRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<ConfirmAndSendProductHandler> _logger;

        public ConfirmAndSendProductHandler(IEscrowProcessRepository escrowProcessRepository,
                                            IEscrowRepository escrowRepository,
                                            IStoreRepository storeRepository,
                                            IUserRepository userRepository,
                                            ILogRepository logRepository,
                                            IConfiguration configuration,
                                            IOrderItemRepository orderItemRepository,
                                            IOrderRepository orderRepository,
                                            IUserProfileRepository userProfileRepository,
                                            ILogger<ConfirmAndSendProductHandler> logger)
        {
            _escrowProcessRepository = escrowProcessRepository;
            _escrowRepository = escrowRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _configuration = configuration;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(ConfirmAndSendProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling ConfirmAndSendProductCommand for SellerId: {SellerId}, EscrowId: {EscrowId}, StoreId: {StoreId}", request.SellerId, request.EscrowId, request.StoreId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found.", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy người bán.");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogWarning("Store with ID {StoreId} not found.", request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng.");
            }
            if (store.UserId != request.SellerId)
            {
                _logger.LogWarning("Store with ID {StoreId} does not belong to SellerId {SellerId}.", request.StoreId, request.SellerId);
                return ServiceResult<string>.Error("Cửa hàng không thuộc về người bán này.");
            }
            var escrow = await _escrowRepository.GetByIdAsync(request.EscrowId);
            if (escrow == null)
            {
                _logger.LogWarning("Escrow with ID {EscrowId} not found.", request.EscrowId);
                return ServiceResult<string>.Error("Không tìm thấy giao dịch.");
            }
            var items = await _orderItemRepository.GetByOrderIdAsync(escrow.OrderId);
            if (items == null || !items.Any())
            {
                _logger.LogWarning("No items found for OrderId {OrderId}.", escrow.OrderId);
                return ServiceResult<string>.Error("Không có sản phẩm nào trong đơn hàng.");
            }
            var order = await _orderRepository.GetByIdAsync(escrow.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", escrow.OrderId);
                return ServiceResult<string>.Error("Không tìm thấy đơn hàng.");
            }
            var buyerProfile = await _userProfileRepository.GetByUserIdAsync(order.BuyerId);
            if (buyerProfile == null)
            {
                _logger.LogWarning("Buyer with ID {BuyerId} not found.", order.BuyerId);
                return ServiceResult<string>.Error("Không tìm thấy người mua.");
            }
            string Message = $"Chào {buyerProfile.FullName},\n\n" +
                $"Giao dịch với mã {escrow.EscrowId} đã được xác nhận và sản phẩm đã được gửi đi.\n" +
                $"Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.\n\n" +
                $"Trân trọng,\n" +
                $"Đội ngũ hỗ trợ khách hàng";
            var buyer = await _userRepository.GetByIdAsync(order.BuyerId);
            var sendmailResult = SendEmailToBuyer(buyer.Email, "Thông báo giao dịch thành công", Message);
            if (!sendmailResult)
            {
                _logger.LogWarning("Failed to send email to buyer with ID {BuyerId}.", buyer.UserId);
            }

            var escrowProcess = new EscrowProcess
            {
                EscrowId = escrow.EscrowId,
                EscrowProcessInformation = "Người bán đã xác nhận và gửi sản phẩm",
                ProcessAt = DateTime.Now,
                ProcessName = "ConfirmAndSendProduct",
                ProcessId = Guid.NewGuid().ToString(),
            };
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = request.SellerId,
                Action = "ConfirmAndSendProduct",
                CreatedAt = DateTime.UtcNow,
                TargetId = escrow.EscrowId,
                TargetType = "Escrow",
            };
            try
            {
                escrow.Expired = DateTime.Now.AddDays(30);
                await _escrowRepository.UpdateAsync(escrow);
                await _escrowProcessRepository.AddAsync(escrowProcess);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Xác nhận và gửi sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming and sending product for EscrowId: {EscrowId}", request.EscrowId);
                return ServiceResult<string>.Error("Lỗi khi xác nhận và gửi sản phẩm.");
            }
        }
        private bool SendEmailToBuyer(string buyerEmail, string subject, string message)
        {

            _logger.LogInformation("Sending email to {BuyerEmail} with subject: {Subject}", buyerEmail, subject);

            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Mail:From"]!));
                email.To.Add(MailboxAddress.Parse(buyerEmail));
                email.Subject = "Thông báo giao dịch thành công";
                email.Body = new TextPart("plain")
                {
                    Text = message
                };
                using var smtp = new SmtpClient();
                smtp.Connect(_configuration["Mail:SmtpHost"],
                int.Parse(_configuration["Mail:SmtpPort"]!),
                MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_configuration["Mail:Username"], _configuration["Mail:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);
                _logger.LogInformation("Email sent successfully to {BuyerEmail}", buyerEmail);
                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while sending email to {BuyerEmail}", buyerEmail);
                return false;
            }
        }
    }
}
