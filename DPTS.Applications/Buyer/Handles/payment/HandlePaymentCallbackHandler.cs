using DPTS.Applications.Buyer.Queries.payment;
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace DPTS.Applications.Buyer.Handles.payment
{
    public class HandlePaymentCallbackHandler : IRequestHandler<HandlePaymentCallbackQuery, ServiceResult<string>>
    {
        private readonly IConfiguration _config;

        public HandlePaymentCallbackHandler(IConfiguration config)
        {
            _config = config;
        }

        public Task<ServiceResult<string>> Handle(HandlePaymentCallbackQuery request, CancellationToken cancellationToken)
        {
            var inputData = new SortedList<string, string>();
            var queryParams = request.GetType().GetProperties();

            string secureHash = string.Empty;
            foreach (var prop in queryParams)
            {
                var name = prop.Name;
                var value = prop.GetValue(request)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    if (name == "vnp_SecureHash")
                    {
                        secureHash = value;
                    }
                    else if (name.StartsWith("vnp_"))
                    {
                        inputData.Add(name, value);
                    }
                }
            }

            var rawData = string.Join("&", inputData.Select(x => $"{x.Key}={x.Value}"));
            var hashSecret = _config["VnPay:HashSecret"];
            var computedHash = HmacSHA512(hashSecret, rawData);

            if (!secureHash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase))
                return Task.FromResult(ServiceResult<string>.Error("Sai chữ ký hash"));

            if (request.vnp_ResponseCode == "00")
            {
                // Thành công → có thể cập nhật DB tại đây
                return Task.FromResult(ServiceResult<string>.Success("Giao dịch thành công"));
            }

            return Task.FromResult(ServiceResult<string>.Error($"Giao dịch thất bại: {request.vnp_ResponseCode}"));
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
