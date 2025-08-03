using DPTS.Applications.Buyer.Queries.wallet;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace DPTS.Applications.Buyer.Handles.wallet
{
    public class AddPaymentMethodHandler : IRequestHandler<AddPaymentMethodCommand, ServiceResult<string>>
    {
        private readonly ILogger<AddPaymentMethodHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserSecurityRepository _userSecurityRepository;

        public AddPaymentMethodHandler(ILogger<AddPaymentMethodHandler> logger, IUserRepository userRepository, IPaymentMethodRepository paymentMethodRepository, ILogRepository logRepository,
        IUserSecurityRepository userSecurityRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _configuration = configuration;
            _paymentMethodRepository = paymentMethodRepository;
            _userSecurityRepository = userSecurityRepository;
        }
        public async Task<ServiceResult<string>> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling add new payment method");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }
            var paymendMethod = new PaymentMethod
            {
                PaymentMethodId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Provider = request.AddPaymentCommand.PaymentProvider,
                IsDefault = false
            };
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Action = "Người dùng vừa thêm phương thức thanh toán",
                Description = $"{request.UserId} vừa thêm phương thức thanh toán",
                CreatedAt = DateTime.UtcNow,
            };
            if (request.AddPaymentCommand.IsVerified)
            {
                var security = await _userSecurityRepository.GetByUserIdAsync(request.UserId);
                if (security == null)
                {
                    _logger.LogError("User current is invalid");
                    return ServiceResult<string>.Error("Người dùng không hợp lệ");
                }
                security.TwoFactorSecret = Guid.NewGuid().ToString();
                user.UpdatedAt = DateTime.UtcNow;
                if (SendMail(user.Email, security.TwoFactorSecret))
                {
                    try
                    {
                        await _userSecurityRepository.UpdateAsync(security);
                        await _userRepository.UpdateAsync(user);
                        _logger.LogInformation("Gửi mail xác nhận thành công");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Send mail is fail");
                        return ServiceResult<string>.Error("Gửi mail và cập nhật thông tin không thành công");
                    }
                }

            }
            try
            {
                await _paymentMethodRepository.AddAsync(paymendMethod);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Thêm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when add new payment method");
                return ServiceResult<string>.Error("Thêm phương thức thanh toán không thành công");
            }
        }
        private bool SendMail(string toAddress, string twoFactorSecret)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Mail:From"]!));
                email.To.Add(MailboxAddress.Parse(toAddress));
                email.Subject = "Mã xác thực hai bước từ DPTS";
                email.Body = new TextPart("plain")
                {
                    Text = $"Bạn đã yêu cầu xác thực phương thức thanh toán vừa được thêm vào.\nMã xác thực của bạn là: {twoFactorSecret}"
                };

                using var smtp = new SmtpClient();
                smtp.Connect(
                    _configuration["Mail:SmtpHost"],
                    int.Parse(_configuration["Mail:SmtpPort"]!),
                    MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate(_configuration["Mail:Username"], _configuration["Mail:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                _logger.LogInformation("2FA email sent to {Email}", toAddress);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when sending 2FA email to {Email}", toAddress);
                return false;
            }
        }
    }
}