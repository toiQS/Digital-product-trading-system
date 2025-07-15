using DPTS.Applications.Buyer.Dtos.wallet;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.wallet
{
    public class GetWalletQuery : IRequest<ServiceResult<WalletDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
